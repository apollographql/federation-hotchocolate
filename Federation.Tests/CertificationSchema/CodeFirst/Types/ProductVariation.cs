using HotChocolate.Types.Relay;

namespace ApolloGraphQL.HotChocolate.Federation.CertificationSchema.CodeFirst.Types;

public class ProductVariation
{
    public ProductVariation(string id)
    {
        Id = id;
    }

    [ID]
    public string Id { get; }
}
