namespace Products;

public class Query
{
    public Product? GetProduct(string id, Data repository)
        => repository.Products.FirstOrDefault(t => t.Id.Equals(id));

    public DeprecatedProduct? GetDeprecatedProduct(string sku, string package, Data repository)
        => repository.DeprecatedProducts.FirstOrDefault(t => t.Sku.Equals(sku) && t.Package.Equals(package));
}

public class QueryType : ObjectType<Query>
{
    protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
    {
        descriptor.Field(q => q.GetProduct(default!, default!))
            .Argument("id", a => a.Type<NonNullType<IdType>>())
            .Type<ProductType>();
        descriptor.Field(q => q.GetDeprecatedProduct(default!, default!, default!))
            .Type<DeprecatedProductType>()
            .Deprecated("Use product query instead");
    }
}