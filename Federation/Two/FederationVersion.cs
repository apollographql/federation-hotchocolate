namespace ApolloGraphQL.HotChocolate.Federation.Two;

/// <summary>
/// Enum defining all supported Apollo Federation v2 versions.
/// </summary>
public enum FederationVersion
{
    FEDERATION_20,
    FEDERATION_21,
    FEDERATION_22,
    FEDERATION_23,
    FEDERATION_24,
    FEDERATION_25,
    // TODO: actually add @policy support
    FEDERATION_26,
    FEDERATION_27,
}