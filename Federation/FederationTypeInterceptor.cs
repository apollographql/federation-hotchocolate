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
using ApolloGraphQL.HotChocolate.Federation.Two;

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

        ApplyFederationDirectives(definition);
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
        if (definition is ObjectTypeDefinition objectTypeDefinition &&
                _context.Options.QueryTypeName == definition.Name)
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

    /// <summary>
    /// Apply Apollo Federation directives based on the custom attributes.
    /// </summary>
    /// <param name="definition">
    /// Type definition
    /// </param>
    private void ApplyFederationDirectives(DefinitionBase? definition)
    {
        switch (definition)
        {
            case EnumTypeDefinition enumTypeDefinition:
                {
                    ApplyEnumDirectives(enumTypeDefinition);
                    ApplyEnumValueDirectives(enumTypeDefinition.Values);
                    break;
                }
            case InterfaceTypeDefinition interfaceTypeDefinition:
                {
                    ApplyInterfaceDirectives(interfaceTypeDefinition);
                    ApplyInterfaceFieldDirectives(interfaceTypeDefinition.Fields);
                    break;
                }
            case InputObjectTypeDefinition inputObjectTypeDefinition:
                {
                    ApplyInputObjectDirectives(inputObjectTypeDefinition);
                    ApplyInputFieldDirectives(inputObjectTypeDefinition.Fields);
                    break;
                }
            case ObjectTypeDefinition objectTypeDefinition:
                {
                    ApplyObjectDirectives(objectTypeDefinition);
                    ApplyObjectFieldDirectives(objectTypeDefinition.Fields);
                    break;
                }
            case UnionTypeDefinition unionTypeDefinition:
                {
                    ApplyUnionDirectives(unionTypeDefinition);
                    break;
                }
            default:
                break;
        }
    }

    private void ApplyEnumDirectives(EnumTypeDefinition enumTypeDefinition)
    {
        var requiresScopes = new List<List<Scope>>();
        var descriptor = EnumTypeDescriptor.From(_context, enumTypeDefinition);
        foreach (var attribute in enumTypeDefinition.RuntimeType.GetCustomAttributes(true))
        {

            switch (attribute)
            {
                case ApolloTagAttribute tag:
                    {
                        descriptor.ApolloTag(tag.Name);
                        break;
                    }
                case ApolloAuthenticatedAttribute:
                    {
                        descriptor.ApolloAuthenticated();
                        break;
                    }
                case InaccessibleAttribute:
                    {
                        descriptor.Inaccessible();
                        break;
                    }
                case RequiresScopesAttribute scopes:
                    {
                        requiresScopes.Add(scopes.Scopes.Select(scope => new Scope(scope)).ToList());
                        break;
                    }
                default: break;
            }
        }

        if (requiresScopes.Count() > 0)
        {
            descriptor.RequiresScopes(requiresScopes);
        }
    }

    private void ApplyEnumValueDirectives(IList<EnumValueDefinition> enumValues)
    {
        foreach (EnumValueDefinition enumValueDefinition in enumValues)
        {
            if (enumValueDefinition.Member != null)
            {
                var enumValueDescriptor = EnumValueDescriptor.From(_context, enumValueDefinition);
                foreach (var attribute in enumValueDefinition.Member.GetCustomAttributes(true))
                {
                    if (attribute is InaccessibleAttribute)
                    {
                        enumValueDescriptor.Inaccessible();
                    }
                    else if (attribute is ApolloTagAttribute casted)
                    {
                        enumValueDescriptor.ApolloTag(casted.Name);
                    }
                }
            }
        }
    }

    private void ApplyInterfaceDirectives(InterfaceTypeDefinition interfaceTypeDefinition)
    {
        var descriptor = InterfaceTypeDescriptor.From(_context, interfaceTypeDefinition);
        var requiresScopes = new List<List<Scope>>();
        foreach (var attribute in interfaceTypeDefinition.RuntimeType.GetCustomAttributes(true))
        {
            switch (attribute)
            {
                case ApolloTagAttribute tag:
                    {
                        descriptor.ApolloTag(tag.Name);
                        break;
                    }
                case ApolloAuthenticatedAttribute:
                    {
                        descriptor.ApolloAuthenticated();
                        break;
                    }
                case InaccessibleAttribute:
                    {
                        descriptor.Inaccessible();
                        break;
                    }
                case RequiresScopesAttribute scopes:
                    {
                        requiresScopes.Add(scopes.Scopes.Select(scope => new Scope(scope)).ToList());
                        break;
                    }
                default: break;
            }
        }

        if (requiresScopes.Count() > 0)
        {
            descriptor.RequiresScopes(requiresScopes);
        }
    }

    private void ApplyInterfaceFieldDirectives(IList<InterfaceFieldDefinition> fields)
    {
        foreach (InterfaceFieldDefinition fieldDefinition in fields)
        {
            var descriptor = InterfaceFieldDescriptor.From(_context, fieldDefinition);
            if (fieldDefinition.Member != null)
            {
                var requiresScopes = new List<List<Scope>>();
                foreach (var attribute in fieldDefinition.Member.GetCustomAttributes(true))
                {
                    switch (attribute)
                    {
                        case ApolloTagAttribute tag:
                            {
                                descriptor.ApolloTag(tag.Name);
                                break;
                            }
                        case ApolloAuthenticatedAttribute:
                            {
                                descriptor.ApolloAuthenticated();
                                break;
                            }
                        case InaccessibleAttribute:
                            {
                                descriptor.Inaccessible();
                                break;
                            }
                        case RequiresScopesAttribute scopes:
                            {
                                requiresScopes.Add(scopes.Scopes.Select(scope => new Scope(scope)).ToList());
                                break;
                            }
                        default: break;
                    }
                }

                if (requiresScopes.Count() > 0)
                {
                    descriptor.RequiresScopes(requiresScopes);
                }
            }
        }
    }

    private void ApplyInputObjectDirectives(InputObjectTypeDefinition inputObjectTypeDefinition)
    {
        var descriptor = InputObjectTypeDescriptor.From(_context, inputObjectTypeDefinition);
        foreach (var attribute in inputObjectTypeDefinition.RuntimeType.GetCustomAttributes(true))
        {
            if (attribute is InaccessibleAttribute)
            {
                descriptor.Inaccessible();
            }
            else if (attribute is ApolloTagAttribute casted)
            {
                descriptor.ApolloTag(casted.Name);
            }

        }
    }

    private void ApplyInputFieldDirectives(IList<InputFieldDefinition> inputFields)
    {
        foreach (InputFieldDefinition fieldDefinition in inputFields)
        {
            if (fieldDefinition.RuntimeType != null)
            {
                var fieldDescriptor = InputFieldDescriptor.From(_context, fieldDefinition);
                foreach (var attribute in fieldDefinition.RuntimeType.GetCustomAttributes(true))
                {
                    if (attribute is InaccessibleAttribute)
                    {
                        fieldDescriptor.Inaccessible();
                    }
                    else if (attribute is ApolloTagAttribute casted)
                    {
                        fieldDescriptor.ApolloTag(casted.Name);
                    }
                }
            }
        }
    }

    private void ApplyObjectDirectives(ObjectTypeDefinition objectTypeDefinition)
    {
        var descriptor = ObjectTypeDescriptor.From(_context, objectTypeDefinition);
        var requiresScopes = new List<List<Scope>>();
        foreach (var attribute in objectTypeDefinition.RuntimeType.GetCustomAttributes(true))
        {
            switch (attribute)
            {
                case ApolloTagAttribute tag:
                    {
                        descriptor.ApolloTag(tag.Name);
                        break;
                    }
                case ApolloAuthenticatedAttribute:
                    {
                        descriptor.ApolloAuthenticated();
                        break;
                    }
                case InaccessibleAttribute:
                    {
                        descriptor.Inaccessible();
                        break;
                    }
                case RequiresScopesAttribute scopes:
                    {
                        requiresScopes.Add(scopes.Scopes.Select(scope => new Scope(scope)).ToList());
                        break;
                    }
                case ShareableAttribute:
                    {
                        descriptor.Shareable();
                        break;
                    }
                default: break;
            }
        }

        if (requiresScopes.Count() > 0)
        {
            descriptor.RequiresScopes(requiresScopes);
        }
    }

    private void ApplyObjectFieldDirectives(IList<ObjectFieldDefinition> fields)
    {
        foreach (ObjectFieldDefinition fieldDefinition in fields)
        {
            if (fieldDefinition.Member != null)
            {
                var requiresScopes = new List<List<Scope>>();
                var descriptor = ObjectFieldDescriptor.From(_context, fieldDefinition);
                foreach (var attribute in fieldDefinition.Member.GetCustomAttributes(true))
                {
                    switch (attribute)
                    {
                        case ApolloTagAttribute tag:
                            {
                                descriptor.ApolloTag(tag.Name);
                                break;
                            }
                        case ApolloAuthenticatedAttribute:
                            {
                                descriptor.ApolloAuthenticated();
                                break;
                            }
                        case InaccessibleAttribute:
                            {
                                descriptor.Inaccessible();
                                break;
                            }
                        case RequiresScopesAttribute scopes:
                            {
                                requiresScopes.Add(scopes.Scopes.Select(scope => new Scope(scope)).ToList());
                                break;
                            }
                        case ShareableAttribute:
                            {
                                descriptor.Shareable();
                                break;
                            }
                        default: break;
                    }
                }

                if (requiresScopes.Count() > 0)
                {
                    descriptor.RequiresScopes(requiresScopes);
                }
            }
        }
    }

    private void ApplyUnionDirectives(UnionTypeDefinition unionTypeDefinition)
    {
        var descriptor = UnionTypeDescriptor.From(_context, unionTypeDefinition);
        foreach (var attribute in unionTypeDefinition.RuntimeType.GetCustomAttributes(true))
        {
            if (attribute is InaccessibleAttribute)
            {
                descriptor.Inaccessible();
            }
            else if (attribute is ApolloTagAttribute casted)
            {
                descriptor.ApolloTag(casted.Name);
            }
        }

    }
}