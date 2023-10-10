using HotChocolate.Types.Relay;

namespace ApolloGraphQL.Federation.HotChocolate.CertificationSchema.AnnotationBased.Types;

public class ProductVariation
{
    public ProductVariation(string id)
    {
        Id = id;
    }

    [ID]
    public string Id { get; }
}
