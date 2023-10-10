using ApolloGraphQL.HotChocolate.Federation;
using ApolloGraphQL.HotChocolate.Federation.Two;
using AnyType = ApolloGraphQL.HotChocolate.Federation.AnyType;
using KeyV2 = ApolloGraphQL.HotChocolate.Federation.Two.KeyDirectiveType;
using ProvidesV2 = ApolloGraphQL.HotChocolate.Federation.Two.ProvidesDirectiveType;
using RequiresV2 = ApolloGraphQL.HotChocolate.Federation.Two.RequiresDirectiveType;

namespace HotChocolate;

/// <summary>
/// Provides extensions to <see cref="ISchemaBuilder"/>.
/// </summary>
public static class ApolloFederationSchemaBuilderExtensionsV2
{
    /// <summary>
    /// Adds support for Apollo Federation to the schema.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="ISchemaBuilder"/>.
    /// </param>
    /// <returns>
    /// Returns the <see cref="ISchemaBuilder"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="builder"/> is <c>null</c>.
    /// </exception>
    public static ISchemaBuilder AddApolloFederationV2(
    this ISchemaBuilder builder)
    {
        return AddApolloFederationV2(builder, new FederatedSchema());
    }

    public static ISchemaBuilder AddApolloFederationV2(
        this ISchemaBuilder builder, FederatedSchema schema)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.SetSchema(schema);

        // scalars
        builder.AddType<AnyType>();
        builder.AddType<FieldSetType>();

        // types
        builder.AddType<EntityType>();
        builder.AddType(new ServiceType(true));

        // directives
        builder.AddType<ComposeDirectiveType>();
        builder.AddType<ExtendsDirectiveType>();
        builder.AddType<ExternalDirectiveType>();
        builder.AddType<InaccessibleDirectiveType>();
        builder.AddType<InterfaceObjectirectiveType>();
        builder.AddType<KeyV2>();
        builder.AddType<LinkDirectiveType>();
        builder.AddType<OverrideDirectiveType>();
        builder.AddType<ProvidesV2>();
        builder.AddType<RequiresV2>();
        builder.AddType<ShareableDirectiveType>();
        builder.AddType<TagDirectiveType>();
        builder.TryAddTypeInterceptor<FederationTypeInterceptor>();
        return builder;
    }
}
