using HotChocolate;
using HotChocolate.Types;
using System.Linq;

namespace ApolloGraphQL.HotChocolate.Federation.CertificationSchema.AnnotationBased.Types;

[ExtendServiceType]
public class Query
{
    public Product? GetProduct([GraphQLType(typeof(IdType))][GraphQLNonNullType] string id, Data repository)
        => repository.Products.FirstOrDefault(t => t.Id.Equals(id));
}
