using ApolloGraphQL.HotChocolate.Federation.Constants;
using ApolloGraphQL.HotChocolate.Federation.Descriptors;
using HotChocolate.Language;
using static ApolloGraphQL.HotChocolate.Federation.Constants.WellKnownContextData;
using static ApolloGraphQL.HotChocolate.Federation.Properties.FederationResources;
using System.Collections.Generic;

namespace HotChocolate.Types;

/// <summary>
/// Provides extensions for type system descriptors.
/// </summary>
public static partial class ApolloFederationDescriptorExtensions
{
    /// <summary>
    /// Adds the @external directive which is used to mark a field as owned by another service.
    /// This allows service A to use fields from service B while also knowing at runtime
    /// the types of that field.
    ///
    /// <example>
    /// # extended from the Users service
    /// extend type User @key(fields: "email") {
    ///   email: String @external
    ///   reviews: [Review]
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
    public static IObjectFieldDescriptor External(
        this IObjectFieldDescriptor descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        return descriptor.Directive(WellKnownTypeNames.External);
    }

    public static IObjectFieldDescriptor InaccessibleField(
        this IObjectFieldDescriptor descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        return descriptor.Directive(WellKnownTypeNames.Inaccessible);
    }


    public static IObjectTypeDescriptor InaccessibleType(
        this IObjectTypeDescriptor descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        return descriptor.Directive(WellKnownTypeNames.Inaccessible);
    }

    /// <summary>
    /// Adds the @key directive which is used to indicate a combination of fields that
    /// can be used to uniquely identify and fetch an object or interface.
    /// <example>
    /// type Product @key(fields: "upc") {
    ///   upc: UPC!
    ///   name: String
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
    public static IEntityResolverDescriptor Key(
        this IObjectTypeDescriptor descriptor,
        string fieldSet,
        bool? resolvable = null)
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

        List<ArgumentNode> arguments = new List<ArgumentNode> {
            new ArgumentNode(
                WellKnownArgumentNames.Fields,
                new StringValueNode(fieldSet)
            )
        };
        if (false == resolvable)
        {
            arguments.Add(
                new ArgumentNode(
                    WellKnownArgumentNames.Resolvable,
                    new BooleanValueNode(false)
                )
            );
        }
        descriptor.Directive(
            WellKnownTypeNames.Key,
            arguments.ToArray());

        return new EntityResolverDescriptor<object>(descriptor);
    }

    /// <summary>
    /// Adds the @requires directive which is used to annotate the required
    /// input fieldset from a base type for a resolver. It is used to develop
    /// a query plan where the required fields may not be needed by the client, but the
    /// service may need additional information from other services.
    ///
    /// <example>
    /// # extended from the Users service
    /// extend type User @key(fields: "id") {
    ///   id: ID! @external
    ///   email: String @external
    ///   reviews: [Review] @requires(fields: "email")
    /// }
    /// </example>
    /// </summary>
    /// <param name="descriptor">
    /// The object field descriptor on which this directive shall be annotated.
    /// </param>
    /// <param name="fieldSet">
    /// The <paramref name="fieldSet"/> describes which fields may
    /// not be needed by the client, but are required by
    /// this service as additional information from other services.
    /// Grammatically, a field set is a selection set minus the braces.
    /// </param>
    /// <returns>
    /// Returns the object field descriptor.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="descriptor"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="fieldSet"/> is <c>null</c> or <see cref="string.Empty"/>.
    /// </exception>
    public static IObjectFieldDescriptor Requires(
        this IObjectFieldDescriptor descriptor,
        string fieldSet)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        if (string.IsNullOrEmpty(fieldSet))
        {
            throw new ArgumentException(
                FieldDescriptorExtensions_Requires_FieldSet_CannotBeNullOrEmpty,
                nameof(fieldSet));
        }

        return descriptor.Directive(
            WellKnownTypeNames.Requires,
            new ArgumentNode(
                WellKnownArgumentNames.Fields,
                new StringValueNode(fieldSet)));
    }

    /// <summary>
    /// Adds the @provides directive which is used to annotate the expected returned
    /// fieldset from a field on a base type that is guaranteed to be selectable by
    /// the gateway.
    ///
    /// <example>
    /// # extended from the Users service
    /// type Review @key(fields: "id") {
    ///     product: Product @provides(fields: "name")
    /// }
    ///
    /// extend type Product @key(fields: "upc") {
    ///     upc: String @external
    ///     name: String @external
    /// }
    /// </example>
    /// </summary>
    /// <param name="descriptor">
    /// The object field descriptor on which this directive shall be annotated.
    /// </param>
    /// <param name="fieldSet">
    /// The fields that are guaranteed to be selectable by the gateway.
    /// Grammatically, a field set is a selection set minus the braces.
    /// </param>
    /// <returns>
    /// Returns the object field descriptor.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="descriptor"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="fieldSet"/> is <c>null</c> or <see cref="string.Empty"/>.
    /// </exception>
    public static IObjectFieldDescriptor Provides(
        this IObjectFieldDescriptor descriptor,
        string fieldSet)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        if (string.IsNullOrEmpty(fieldSet))
        {
            throw new ArgumentException(
                FieldDescriptorExtensions_Provides_FieldSet_CannotBeNullOrEmpty,
                nameof(fieldSet));
        }

        return descriptor.Directive(
            WellKnownTypeNames.Provides,
            new ArgumentNode(
                WellKnownArgumentNames.Fields,
                new StringValueNode(fieldSet)));
    }

    /// <summary>
    /// Mark the type as an extension
    /// of a type that is defined by another service when
    /// using apollo federation.
    /// </summary>
    [Obsolete("Use ExtendsType type instead")]
    public static IObjectTypeDescriptor ExtendServiceType(
        this IObjectTypeDescriptor descriptor)
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

    public static IObjectFieldDescriptor Override(
        this IObjectFieldDescriptor descriptor,
        string from)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        if (string.IsNullOrEmpty(from))
        {
            throw new ArgumentException(
                FieldDescriptorExtensions_Override_From_CannotBeNullOrEmpty,
                nameof(from));
        }

        return descriptor.Directive(
            WellKnownTypeNames.Override,
            new ArgumentNode(
                WellKnownArgumentNames.From,
                new StringValueNode(from)));
    }


    public static IObjectTypeDescriptor InterfaceObject(this IObjectTypeDescriptor descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        return descriptor.Directive(WellKnownTypeNames.InterfaceObject);
    }

    /// <summary>
    /// Adds @extends directive which is used to represent type extensions in the schema. Federated extended types should have 
    /// corresponding @key directive defined that specifies primary key required to fetch the underlying object.
    ///
    /// <example>
    /// # extended from the Users service
    /// type User @extends @key(fields: "email") {
    ///   email: String @external
    ///   reviews: [Review]
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
    public static IObjectTypeDescriptor ExtendsType(
        this IObjectTypeDescriptor descriptor)
    {
        if (descriptor is null)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        return descriptor.Directive(WellKnownTypeNames.Extends);
    }
}
