namespace Products;

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
