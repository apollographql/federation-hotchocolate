namespace Products;

public class DeprecatedProduct
{
    public DeprecatedProduct(string sku, string package, string? reason, User? createdBy)
    {
        Sku = sku;
        Package = package;
        Reason = reason;
        CreatedBy = createdBy;
    }

    public string Sku { get; }

    public string Package { get; }

    public string? Reason { get; }

    public User? CreatedBy { get; }
}

class DeprecatedProductType : ObjectType<DeprecatedProduct>
{
    protected override void Configure(IObjectTypeDescriptor<DeprecatedProduct> descriptor)
    {
        descriptor.BindFieldsImplicitly();
        descriptor.Key("sku package")
            .ResolveReferenceWith(t => GetProductByPackage(default!, default!, default!));
    }

    private static DeprecatedProduct? GetProductByPackage(
        string sku,
        string package,
        Data repository)
        => repository.DeprecatedProducts.FirstOrDefault(
            t => t.Sku.Equals(sku) &&
                t.Package.Equals(package));
}