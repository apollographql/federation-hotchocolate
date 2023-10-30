namespace Products;

public class ProductDimension
{
    public ProductDimension(string size, double weight, string? unit)
    {
        Size = size;
        Weight = weight;
        Unit = unit;
    }

    public string? Size { get; }

    public double? Weight { get; }

    public string? Unit { get; }
}

public class ProductDimensionType : ObjectType<ProductDimension>
{
    protected override void Configure(IObjectTypeDescriptor<ProductDimension> descriptor)
    {
        descriptor.BindFieldsImplicitly();
        descriptor.Shareable();
        descriptor.Field(pd => pd.Unit).Inaccessible();
    }
}
