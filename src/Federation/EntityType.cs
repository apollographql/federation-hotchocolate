using ApolloGraphQL.Federation.HotChocolate.Properties;
using static ApolloGraphQL.Federation.HotChocolate.Constants.WellKnownTypeNames;

namespace ApolloGraphQL.Federation.HotChocolate;

/// <summary>
/// A union called _Entity which is a union of all types that use the @key directive,
/// including both types native to the schema and extended types.
/// </summary>
public sealed class EntityType : UnionType
{
    protected override void Configure(IUnionTypeDescriptor descriptor)
        => descriptor
            .Name(Entity)
            .Description(FederationResources.EntityType_Description);
}
