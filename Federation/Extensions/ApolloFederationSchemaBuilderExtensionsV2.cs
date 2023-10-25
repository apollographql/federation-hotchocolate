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
    public static ISchemaBuilder AddApolloFederationV2(this ISchemaBuilder builder)
    {
        return AddApolloFederationV2(builder, new FederatedSchema());
    }

    /// <summary>
    /// Adds support for Apollo Federation to the schema.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="ISchemaBuilder"/>.
    /// </param>
    /// <returns>
    /// Returns the <see cref="ISchemaBuilder"/>.
    /// </returns>
    /// <param name="version">
    /// Target Federation version
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="builder"/> is <c>null</c>.
    /// </exception>
    public static ISchemaBuilder AddApolloFederationV2(this ISchemaBuilder builder, FederationVersion version)
    {
        return AddApolloFederationV2(builder, new FederatedSchema(version));
    }

    /// <summary>
    /// Adds support for Apollo Federation to the schema.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="ISchemaBuilder"/>.
    /// </param>
    /// <param name="schema">
    /// Federated schema object.
    /// </param>
    /// <returns>
    /// Returns the <see cref="ISchemaBuilder"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="builder"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="schema"/> is <c>null</c>.
    /// </exception>
    public static ISchemaBuilder AddApolloFederationV2(this ISchemaBuilder builder, FederatedSchema schema)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }
        if (schema is null)
        {
            throw new ArgumentNullException(nameof(schema));
        }
        // disable hot chocolate tag directive
        // specify default Query type name if not specified
        builder.ModifyOptions(opt =>
        {
            opt.EnableTag = false;
            if (opt.QueryTypeName is null)
            {
                opt.QueryTypeName = "Query";
            }
        });

        builder.SetSchema(schema);

        // scalars
        builder.AddType<AnyType>();
        builder.AddType<FieldSetType>();

        // types
        builder.AddType<EntityType>();
        builder.AddType(new ServiceType(true));

        // directives
        switch (schema.FederationVersion)
        {
            case FederationVersion.FEDERATION_25:
                {
                    builder.AddType<ScopeType>();
                    builder.AddType<AuthenticatedDirectiveType>();
                    builder.AddType<RequiresScopesDirectiveType>();
                    builder.BindRuntimeType<Scope, ScopeType>();
                    goto case FederationVersion.FEDERATION_24;
                }
            case FederationVersion.FEDERATION_24: // same as 2.3
            case FederationVersion.FEDERATION_23:
                {
                    builder.AddType<InterfaceObjectirectiveType>();
                    goto case FederationVersion.FEDERATION_22;
                }
            case FederationVersion.FEDERATION_22: // same as 2.1
            case FederationVersion.FEDERATION_21:
                {
                    builder.AddType<ComposeDirectiveType>();
                    goto case FederationVersion.FEDERATION_20;
                }
            case FederationVersion.FEDERATION_20:
                {
                    builder.AddType<ExtendsDirectiveType>();
                    builder.AddType<ExternalDirectiveType>();
                    builder.AddType<InaccessibleDirectiveType>();
                    builder.AddType<KeyV2>();
                    builder.AddType<LinkDirectiveType>();
                    builder.AddType<OverrideDirectiveType>();
                    builder.AddType<ProvidesV2>();
                    builder.AddType<RequiresV2>();
                    builder.AddType<ShareableDirectiveType>();
                    builder.AddType<TagDirectiveType>();
                    break;
                }
            default:
                {
                    break;
                }
        }
        builder.TryAddTypeInterceptor<FederationTypeInterceptor>();
        return builder;
    }
}
