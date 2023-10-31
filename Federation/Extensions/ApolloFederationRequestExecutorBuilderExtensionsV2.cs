using ApolloGraphQL.HotChocolate.Federation.Two;
using HotChocolate.Execution.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extensions to <see cref="IRequestExecutorBuilder"/>.
/// </summary>
public static class ApolloFederationRequestExecutorBuilderExtensionsV2
{
    /// <summary>
    /// Adds support for Apollo Federation V2 to the schema.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IRequestExecutorBuilder"/>.
    /// </param>
    /// <param name="version">
    /// Target Federation version
    /// </param
    /// <returns>
    /// Returns the <see cref="IRequestExecutorBuilder"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="builder"/> is <c>null</c>.
    /// </exception>
    public static IRequestExecutorBuilder AddApolloFederationV2(
        this IRequestExecutorBuilder builder,
        FederationVersion version = FederationVersion.FEDERATION_25)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.ConfigureSchema(s => ApolloFederationSchemaBuilderExtensionsV2.AddApolloFederationV2(s, version));
    }

    /// <summary>
    /// Adds support for Apollo Federation V2 to the schema.
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IRequestExecutorBuilder"/>.
    /// </param>
    /// <param name="schema">
    /// Federated schema object
    /// </param
    /// <returns>
    /// Returns the <see cref="IRequestExecutorBuilder"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="builder"/> is <c>null</c>.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="schema"/> is <c>null</c>.
    /// </exception>
    public static IRequestExecutorBuilder AddApolloFederationV2(
        this IRequestExecutorBuilder builder,
        FederatedSchema schema)
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (schema is null)
        {
            throw new ArgumentNullException(nameof(schema));
        }

        return builder.ConfigureSchema(s => ApolloFederationSchemaBuilderExtensionsV2.AddApolloFederationV2(s, schema));
    }

    /// <summary>
    /// Applies @contact directive on the schema
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IRequestExecutorBuilder"/>.
    /// </param>
    /// <param name="name">
    /// Contact title of the subgraph owner
    /// </param>
    /// <param name="url">
    /// URL where the subgraph's owner can be reached
    /// </param>
    /// <param name="description">
    /// Other relevant contact notes; supports markdown links
    /// </param>
    /// <returns>
    /// Returns the <see cref="IRequestExecutorBuilder"/>.
    /// </returns>
    public static IRequestExecutorBuilder ApplyContactDirective(this IRequestExecutorBuilder builder, string name, string? url = null, string? description = null)
    {
        return builder.SetSchema(s => s.Contact(name, url, description));
    }


    /// <summary>
    /// Applies @composeDirective on the schema
    /// </summary>
    /// <param name="builder">
    /// The <see cref="IRequestExecutorBuilder"/>.
    /// </param>
    /// <param name="url"></param>
    /// Url of specification to be imported
    /// <param name="names">
    /// Names of the directives that should be preserved in the supergraph composition.
    /// </param>
    /// <returns>
    /// Returns the <see cref="IRequestExecutorBuilder"/>.
    /// </returns>
    public static IRequestExecutorBuilder ApplyComposeDirective(this IRequestExecutorBuilder builder, string url, params string[] names)
    {
        return builder.SetSchema(s =>
        {
            foreach (string name in names)
            {
                s.ComposeDirective(name);
            }
            s.Link(url, names);
        });
    }
}
