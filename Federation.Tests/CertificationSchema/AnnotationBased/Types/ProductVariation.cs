using HotChocolate.Types.Relay;

namespace ApolloGraphQL.HotChocolate.Federation.CertificationSchema.AnnotationBased.Types;

public class ProductVariation
{
    public ProductVariation(string id)
    {
        Id = id;
    }

    [ID]
    public string Id { get; }
}
