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
    /// <param name="schemaConfiguration">
    /// <see cref="ISchemaTypeDescriptor"/> configuration Action.
    /// </param>
    /// <returns>
    /// Returns the <see cref="IRequestExecutorBuilder"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="builder"/> is <c>null</c>.
    /// </exception>
    public static IRequestExecutorBuilder AddApolloFederationV2(
        this IRequestExecutorBuilder builder,
        FederationVersion version = FederationVersion.FEDERATION_25,
        Action<ISchemaTypeDescriptor>? schemaConfiguration = null
    )
    {
        if (builder is null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return builder.ConfigureSchema(s => ApolloFederationSchemaBuilderExtensionsV2.AddApolloFederationV2(s, version, schemaConfiguration));
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
}
