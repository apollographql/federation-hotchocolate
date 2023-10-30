namespace Products;

public class Inventory
{

    public Inventory(string id)
    {
        Id = id;
    }

    public string Id { get; }

    public List<DeprecatedProduct> DeprecatedProducts(Data repository) => repository.DeprecatedProducts;
}

public class InventoryType : ObjectType<Inventory>
{
    protected override void Configure(IObjectTypeDescriptor<Inventory> descriptor)
    {
        descriptor.BindFieldsImplicitly();
        descriptor.Field(i => i.Id).Type<NonNullType<IdType>>();
        descriptor.Key("id")
            .ResolveReferenceWith(i => GetInventoryById(default!, default!))
            .InterfaceObject();
    }

    private static Inventory? GetInventoryById(
        string id,
        Data repository)
    {
        return repository.Inventories().FirstOrDefault(
            r => r.Id.Equals(id));
    }
}
