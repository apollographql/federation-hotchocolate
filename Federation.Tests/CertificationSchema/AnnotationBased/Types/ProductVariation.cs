using HotChocolate;
using HotChocolate.Types;

namespace ApolloGraphQL.HotChocolate.Federation.CertificationSchema.AnnotationBased.Types;

public class ProductVariation
{
    public ProductVariation(string id)
    {
        Id = id;
    }

    [GraphQLType(typeof(IdType))]
    [GraphQLNonNullType]
    public string Id { get; }
}
