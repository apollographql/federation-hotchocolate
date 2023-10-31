using ApolloGraphQL.HotChocolate.Federation.Constants;
using ApolloGraphQL.HotChocolate.Federation.Descriptors;
using HotChocolate.Language;
using static ApolloGraphQL.HotChocolate.Federation.Properties.FederationResources;
using static ApolloGraphQL.HotChocolate.Federation.Constants.WellKnownContextData;

namespace HotChocolate.Types;

/// <summary>
/// Provides extensions for type system descriptors.
/// </summary>
public static partial class ApolloFederationDescriptorExtensions
{
    /// <summary>
    /// Applies @extends directive which is used to represent type extensions in the schema. Federated extended types should have 
    /// corresponding @key directive defined that specifies primary key required to fetch the underlying object.
    /// 
    /// NOTE: Federation v2 no longer requires `@extends` directive due to the smart entity type merging. All usage of @extends
    /// directive should be removed from your Federation v2 schemas.
    /// <example>
    /// # extended from the Users service
    /// type Foo @extends @key(fields: "id") {
    ///   id: ID
    ///   description: String
    /// }
    /// </example>
    /// </summary>
    /// <param name="descriptor">
    /// The object type descriptor on which this directive shall be annotated.
    /// </param>
    /// <returns>
    /// Returns the object type descriptor.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="descriptor"/> is <c>null</c>.
    /// </exception>
    public static IObjectTypeDescriptor<T> ExtendsType<T>(
        this IObjectTypeDescriptor<T> descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        return descriptor.Directive(WellKnownTypeNames.Extends);
    }

    /// <summary>
    /// Adds the @key directive which is used to indicate a combination of fields that can be used to uniquely
    /// identify and fetch an object or interface. The specified field set can represent single field (e.g. "id"),
    /// multiple fields (e.g. "id name") or nested selection sets (e.g. "id user { name }"). Multiple keys can
    /// be specified on a target type.
    /// <example>
    /// type Foo @key(fields: "id") {
    ///   id: ID!
    ///   field: String
    /// }
    /// </example>
    /// </summary>
    /// <param name="descriptor">
    /// The object type descriptor on which this directive shall be annotated.
    /// </param>
    /// <param name="fieldSet">
    /// The field set that describes the key.
    /// Grammatically, a field set is a selection set minus the braces.
    /// </param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="descriptor"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="fieldSet"/> is <c>null</c> or <see cref="string.Empty"/>.
    /// </exception>
    /// <summary>
    public static IEntityResolverDescriptor<T> Key<T>(
        this IObjectTypeDescriptor<T> descriptor,
        string fieldSet)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        if (string.IsNullOrEmpty(fieldSet))
        {
            throw new ArgumentException(
                FieldDescriptorExtensions_Key_FieldSet_CannotBeNullOrEmpty,
                nameof(fieldSet));
        }

        descriptor.Directive(
            WellKnownTypeNames.Key,
            new ArgumentNode(
                WellKnownArgumentNames.Fields,
                new StringValueNode(fieldSet)));

        return new EntityResolverDescriptor<T>(descriptor);
    }

    /// <summary>
    /// Mark the type as an extension
    /// of a type that is defined by another service when
    /// using apollo federation.
    /// </summary>
    [Obsolete("Use ExtendsType type instead")]
    public static IObjectTypeDescriptor<T> ExtendServiceType<T>(
        this IObjectTypeDescriptor<T> descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        descriptor
            .Extend()
            .OnBeforeCreate(d => d.ContextData[ExtendMarker] = true);

        return descriptor;
    }

    /// <summary>
    /// Applies the @interfaceObject directive which provides meta information to the router that this entity
    /// type defined within this subgraph is an interface in the supergraph. This allows you to extend functionality
    /// of an interface across the supergraph without having to implement (or even be aware of) all its implementing types.
    /// <example>
    /// type Foo @interfaceObject @key(fields: "ids") {
    ///   id: ID!
    ///   newCommonField: String
    /// }
    /// </example>
    /// </summary>
    /// <param name="descriptor">
    /// The object field descriptor on which this directive shall be annotated.
    /// </param>
    /// <returns>
    /// Returns the object field descriptor.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="descriptor"/> is <c>null</c>.
    /// </exception>
    public static IObjectTypeDescriptor<T> InterfaceObject<T>(this IObjectTypeDescriptor<T> descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        return descriptor.Directive(WellKnownTypeNames.InterfaceObject);
    }

    /// <summary>
    /// Applies @shareable directive which indicates that given object and/or field can be resolved by multiple subgraphs. 
    /// If an object is marked as @shareable then all its fields are automatically shareable without the need
    /// for explicitly marking them with @shareable directive. All fields referenced from @key directive are 
    /// automatically shareable as well.
    /// <example>
    /// type Foo @key(fields: "id") {
    ///   id: ID!                           # shareable because id is a key field
    ///   name: String                      # non-shareable
    ///   description: String @shareable    # shareable
    /// }
    ///
    /// type Bar @shareable {
    ///   description: String               # shareable because User is marked shareable
    ///   name: String                      # shareable because User is marked shareable
    /// }
    /// </example>
    /// </summary>
    /// <param name="descriptor">
    /// The object type descriptor on which this directive shall be annotated.
    /// </param>
    /// <returns>
    /// Returns the object type descriptor.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="descriptor"/> is <c>null</c>.
    /// </exception>
    public static IObjectTypeDescriptor<T> Shareable<T>(this IObjectTypeDescriptor<T> descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        return descriptor.Directive(WellKnownTypeNames.Shareable);
    }
}
