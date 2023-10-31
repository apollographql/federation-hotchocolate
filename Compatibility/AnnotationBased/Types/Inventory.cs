using ApolloGraphQL.HotChocolate.Federation;

namespace Products;

[Key("id")]
[InterfaceObject]
public class Inventory
{

    public Inventory(string id)
    {
        Id = id;
    }

    [GraphQLType(typeof(IdType))]
    [GraphQLNonNullType]
    public string Id { get; }

    public List<DeprecatedProduct> DeprecatedProducts(Data repository) => repository.DeprecatedProducts;

    [ReferenceResolver]
    public static Inventory? GetInventoryById(
        string id,
        Data repository)
    {
        return repository.Inventories().FirstOrDefault(
            r => r.Id.Equals(id));
    }
}
