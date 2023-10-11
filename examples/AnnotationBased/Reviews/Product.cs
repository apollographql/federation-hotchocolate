using ApolloGraphQL.HotChocolate.Federation;

namespace Reviews;

[Key("upc")]
[ExtendServiceType]
public class Product
{
    public Product(string upc)
    {
        Upc = upc;
    }

    [External]
    public string Upc { get; }

    public Task<IEnumerable<Review>> GetReviews(ReviewRepository repository)
        => repository.GetByProductUpcAsync(Upc);

    [ReferenceResolver]
    public static Product GetByIdAsync(string upc) => new(upc);
}
