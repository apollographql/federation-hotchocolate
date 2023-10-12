using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using ApolloGraphQL.HotChocolate.Federation.Constants;
using ApolloGraphQL.HotChocolate.Federation.Descriptors;
using ApolloGraphQL.HotChocolate.Federation.Helpers;
using HotChocolate.Configuration;
using HotChocolate.Language;
using HotChocolate.Resolvers;
using HotChocolate.Types.Descriptors;
using HotChocolate.Types.Descriptors.Definitions;
using static ApolloGraphQL.HotChocolate.Federation.ThrowHelper;
using static ApolloGraphQL.HotChocolate.Federation.Constants.WellKnownContextData;

namespace ApolloGraphQL.HotChocolate.Federation;

internal sealed class FederationTypeInterceptor : TypeInterceptor
{
    private static readonly object _empty = new();

    private static readonly MethodInfo _matches =
        typeof(ReferenceResolverHelper)
            .GetMethod(
                nameof(ReferenceResolverHelper.Matches),
                BindingFlags.Static | BindingFlags.Public)!;

    private static readonly MethodInfo _execute =
        typeof(ReferenceResolverHelper)
            .GetMethod(
                nameof(ReferenceResolverHelper.ExecuteAsync),
                BindingFlags.Static | BindingFlags.Public)!;

    private static readonly MethodInfo _invalid =
        typeof(ReferenceResolverHelper)
            .GetMethod(
                nameof(ReferenceResolverHelper.Invalid),
                BindingFlags.Static | BindingFlags.Public)!;

    private readonly List<ObjectType> _entityTypes = new();
    private IDescriptorContext _context = default!;
    public override void OnBeforeCreateSchema(IDescriptorContext context, ISchemaBuilder schemaBuilder)
    {
        base.OnBeforeCreateSchema(context, schemaBuilder);
        _context = context;
    }

    public override void OnAfterInitialize(
        ITypeDiscoveryContext discoveryContext,
        DefinitionBase definition)
    {
        if (discoveryContext.Type is ObjectType objectType &&
            definition is ObjectTypeDefinition objectTypeDefinition)
        {
            ApplyMethodLevelReferenceResolvers(
                objectType,
                objectTypeDefinition);

            AddToUnionIfHasTypeLevelKeyDirective(
                objectType,
                objectTypeDefinition);
        }
    }

    public override void OnTypesInitialized()
    {
        if (_entityTypes.Count == 0)
        {
            throw EntityType_NoEntities();
        }
    }

    public override void OnBeforeCompleteType(
        ITypeCompletionContext completionContext,
        DefinitionBase definition)
    {
        AddMemberTypesToTheEntityUnionType(
            completionContext,
            definition);

        AddServiceTypeToQueryType(
            completionContext,
            definition);

        AddFederationDirectiveMarkers(definition);
    }

    public override void OnAfterCompleteType(
        ITypeCompletionContext completionContext,
        DefinitionBase definition)
    {
        if (completionContext.Type is ObjectType type &&
            definition is ObjectTypeDefinition typeDef)
        {
            CompleteExternalFieldSetters(type, typeDef);
            CompleteReferenceResolver(typeDef);
        }
    }

    private void CompleteExternalFieldSetters(ObjectType type, ObjectTypeDefinition typeDef)
        => ExternalSetterExpressionHelper.TryAddExternalSetter(type, typeDef);

    private void CompleteReferenceResolver(ObjectTypeDefinition typeDef)
    {
        if (typeDef.GetContextData().TryGetValue(EntityResolver, out var value) &&
            value is IReadOnlyList<ReferenceResolverDefinition> resolvers)
        {
            if (resolvers.Count == 1)
            {
                typeDef.ContextData[EntityResolver] = resolvers[0].Resolver;
            }
            else
            {
                var expressions = new Stack<(Expression Condition, Expression Execute)>();
                var context = Expression.Parameter(typeof(IResolverContext));

                foreach (var resolverDef in resolvers)
                {
                    Expression required = Expression.Constant(resolverDef.Required);
                    Expression resolver = Expression.Constant(resolverDef.Resolver);
                    Expression condition = Expression.Call(_matches, context, required);
                    Expression execute = Expression.Call(_execute, context, resolver);
                    expressions.Push((condition, execute));
                }

                Expression current = Expression.Call(_invalid, context);
                var variable = Expression.Variable(typeof(ValueTask<object?>));

                while (expressions.Count > 0)
                {
                    var expression = expressions.Pop();
                    current = Expression.IfThenElse(
                        expression.Condition,
                        Expression.Assign(variable, expression.Execute),
                        current);
                }

                current = Expression.Block(new[] { variable }, current, variable);

                typeDef.ContextData[EntityResolver] =
                    Expression.Lambda<FieldResolverDelegate>(current, context).Compile();
            }
        }
    }

    private void AddServiceTypeToQueryType(
        ITypeCompletionContext completionContext,
        DefinitionBase? definition)
    {
        if (completionContext.IsQueryType == true &&
            definition is ObjectTypeDefinition objectTypeDefinition)
        {
            var serviceFieldDescriptor = ObjectFieldDescriptor.New(
                _context,
                WellKnownFieldNames.Service);
            serviceFieldDescriptor
                .Type<NonNullType<ServiceType>>()
                .Resolve(_empty);
            objectTypeDefinition.Fields.Add(serviceFieldDescriptor.CreateDefinition());

            var entitiesFieldDescriptor = ObjectFieldDescriptor.New(
                _context,
                WellKnownFieldNames.Entities);
            entitiesFieldDescriptor
                .Type<NonNullType<ListType<EntityType>>>()
                .Argument(
                    WellKnownArgumentNames.Representations,
                    descriptor => descriptor.Type<NonNullType<ListType<NonNullType<AnyType>>>>())
                .Resolve(
                    c => EntitiesResolver.ResolveAsync(
                        c.Schema,
                        c.ArgumentValue<IReadOnlyList<Representation>>(
                            WellKnownArgumentNames.Representations),
                        c
                    ));
            objectTypeDefinition.Fields.Add(entitiesFieldDescriptor.CreateDefinition());
        }
    }

    private void ApplyMethodLevelReferenceResolvers(
        ObjectType objectType,
        ObjectTypeDefinition objectTypeDefinition)
    {
        if (objectType.RuntimeType != typeof(object))
        {
            var descriptor = ObjectTypeDescriptor.From(_context, objectTypeDefinition);

            foreach (var possibleReferenceResolver in
                objectType.RuntimeType.GetMethods(BindingFlags.Static | BindingFlags.Public))
            {
                if (possibleReferenceResolver.IsDefined(typeof(ReferenceResolverAttribute)))
                {
                    foreach (var attribute in possibleReferenceResolver.GetCustomAttributes(true))
                    {
                        if (attribute is ReferenceResolverAttribute casted)
                        {
                            casted.Configure(_context, descriptor, possibleReferenceResolver);
                        }
                    }
                }
            }
            descriptor.CreateDefinition();
        }
    }

    private void AddToUnionIfHasTypeLevelKeyDirective(
        ObjectType objectType,
        ObjectTypeDefinition objectTypeDefinition)
    {
        if (objectTypeDefinition.Directives.Any(
                d => d.Value is DirectiveNode { Name.Value: WellKnownTypeNames.Key }) ||
            objectTypeDefinition.Fields.Any(f => f.ContextData.ContainsKey(WellKnownTypeNames.Key)))
        {
            _entityTypes.Add(objectType);
        }
    }

    private void AddMemberTypesToTheEntityUnionType(
        ITypeCompletionContext completionContext,
        DefinitionBase? definition)
    {
        if (completionContext.Type is EntityType &&
            definition is UnionTypeDefinition unionTypeDefinition)
        {
            foreach (var objectType in _entityTypes)
            {
                unionTypeDefinition.Types.Add(TypeReference.Create(objectType));
            }
        }
    }

    private void AddFederationDirectiveMarkers(DefinitionBase? definition)
    {
        switch (definition)
        {
            case EnumTypeDefinition enumTypeDefinition:
                {
                    var descriptor = EnumTypeDescriptor.From(_context, enumTypeDefinition);
                    foreach (var attribute in enumTypeDefinition.RuntimeType.GetCustomAttributes(true))
                    {
                        if (attribute is InaccessibleAttribute)
                        {
                            descriptor.Inaccessible();
                        }
                        if (attribute is ApolloTagAttribute casted)
                        {
                            descriptor.ApolloTag(casted.Name);
                        }
                    }

                    foreach (EnumValueDefinition enumValueDefinition in enumTypeDefinition.Values)
                    {
                        var enumValueDescriptor = EnumValueDescriptor.From(_context, enumValueDefinition);
                        if (enumValueDefinition.Member != null)
                        {
                            foreach (var attribute in enumValueDefinition.Member.GetCustomAttributes(true))
                            {
                                if (attribute is InaccessibleAttribute)
                                {
                                    enumValueDescriptor.Inaccessible();
                                }
                                if (attribute is ApolloTagAttribute casted)
                                {
                                    enumValueDescriptor.ApolloTag(casted.Name);
                                }
                            }
                        }
                    }
                    break;
                }
            case InterfaceTypeDefinition interfaceTypeDefinition:
                {
                    var descriptor = InterfaceTypeDescriptor.From(_context, interfaceTypeDefinition);

                    foreach (var attribute in interfaceTypeDefinition.RuntimeType.GetCustomAttributes(true))
                    {
                        if (attribute is InaccessibleAttribute)
                        {
                            descriptor.Inaccessible();
                        }
                        if (attribute is ApolloTagAttribute casted)
                        {
                            descriptor.ApolloTag(casted.Name);
                        }
                    }
                    foreach (InterfaceFieldDefinition fieldDefinition in interfaceTypeDefinition.Fields)
                    {
                        var fieldDescriptor = InterfaceFieldDescriptor.From(_context, fieldDefinition);
                        if (fieldDefinition.Member != null)
                        {
                            foreach (var attribute in fieldDefinition.Member.GetCustomAttributes(true))
                            {
                                if (attribute is InaccessibleAttribute)
                                {
                                    fieldDescriptor.Inaccessible();
                                }
                                if (attribute is ApolloTagAttribute casted)
                                {
                                    fieldDescriptor.ApolloTag(casted.Name);
                                }
                            }
                        }
                    }
                    break;
                }
            case InputObjectTypeDefinition inputObjectTypeDefinition:
                {
                    var descriptor = InputObjectTypeDescriptor.From(_context, inputObjectTypeDefinition);
                    foreach (var attribute in inputObjectTypeDefinition.RuntimeType.GetCustomAttributes(true))
                    {
                        if (attribute is InaccessibleAttribute)
                        {
                            descriptor.Inaccessible();
                        }
                        if (attribute is ApolloTagAttribute casted)
                        {
                            descriptor.ApolloTag(casted.Name);
                        }

                    }
                    foreach (InputFieldDefinition fieldDefinition in inputObjectTypeDefinition.Fields)
                    {
                        var fieldDescriptor = InputFieldDescriptor.From(_context, fieldDefinition);
                        if (fieldDefinition.RuntimeType != null)
                        {
                            foreach (var attribute in fieldDefinition.RuntimeType.GetCustomAttributes(true))
                            {
                                if (attribute is InaccessibleAttribute)
                                {
                                    fieldDescriptor.Inaccessible();
                                }
                                if (attribute is ApolloTagAttribute casted)
                                {
                                    fieldDescriptor.ApolloTag(casted.Name);
                                }
                            }
                        }
                    }
                    break;
                }
            case ObjectTypeDefinition objectTypeDefinition:
                {
                    var descriptor = ObjectTypeDescriptor.From(_context, objectTypeDefinition);

                    foreach (var attribute in objectTypeDefinition.RuntimeType.GetCustomAttributes(true))
                    {
                        if (attribute is InaccessibleAttribute)
                        {
                            descriptor.Inaccessible();
                        }
                        if (attribute is ShareableAttribute)
                        {
                            descriptor.Shareable();
                        }
                        if (attribute is ApolloTagAttribute casted)
                        {
                            descriptor.ApolloTag(casted.Name);
                        }
                    }
                    foreach (ObjectFieldDefinition fieldDefinition in objectTypeDefinition.Fields)
                    {
                        var fieldDescriptor = ObjectFieldDescriptor.From(_context, fieldDefinition);
                        if (fieldDefinition.Member != null)
                        {
                            foreach (var attribute in fieldDefinition.Member.GetCustomAttributes(true))
                            {
                                if (attribute is InaccessibleAttribute)
                                {
                                    fieldDescriptor.Inaccessible();
                                }
                                if (attribute is ShareableAttribute)
                                {
                                    fieldDescriptor.Shareable();
                                }
                                if (attribute is ApolloTagAttribute casted)
                                {
                                    fieldDescriptor.ApolloTag(casted.Name);
                                }
                            }
                        }
                    }
                    break;
                }
            case UnionTypeDefinition unionTypeDefinition:
                {
                    var descriptor = UnionTypeDescriptor.From(_context, unionTypeDefinition);

                    foreach (var attribute in unionTypeDefinition.RuntimeType.GetCustomAttributes(true))
                    {
                        if (attribute is InaccessibleAttribute)
                        {
                            descriptor.Inaccessible();
                        }
                        if (attribute is ApolloTagAttribute casted)
                        {
                            descriptor.ApolloTag(casted.Name);
                        }
                    }
                    break;
                }
            default:
                break;
        }
    }
}