namespace Products;

public class Query
{
    public Product? GetProduct([GraphQLType(typeof(IdType))][GraphQLNonNullType] string id, Data repository)
        => repository.Products.FirstOrDefault(t => t.Id.Equals(id));

    [GraphQLDeprecated("Use product query instead")]
    public DeprecatedProduct? GetDeprecatedProduct(string sku, string package, Data repository)
        => repository.DeprecatedProducts.FirstOrDefault(t => t.Sku.Equals(sku) && t.Package.Equals(package));
}
