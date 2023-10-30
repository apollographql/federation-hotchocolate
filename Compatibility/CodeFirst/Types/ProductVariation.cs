namespace Products;

public class ProductVariation
{
    public ProductVariation(string id)
    {
        Id = id;
    }

    public string Id { get; }
}

public class ProductVariationType : ObjectType<ProductVariation>
{
    protected override void Configure(IObjectTypeDescriptor<ProductVariation> descriptor)
    {
        descriptor.BindFieldsImplicitly();
        descriptor.Field(pv => pv.Id).Type<NonNullType<IdType>>();
    }
}
