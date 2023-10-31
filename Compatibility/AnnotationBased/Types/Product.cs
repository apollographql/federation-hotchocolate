using ApolloGraphQL.HotChocolate.Federation;

namespace Products;

[Key("id")]
[Key("sku package")]
[Key("sku variation { id }")]
[Custom]
public class Product
{
    public Product(string id, string? sku, string? package, ProductVariation? variation, ProductDimension? dimensions, User? createdBy, string? notes, List<ProductResearch> research)
    {
        Id = id;
        Sku = sku;
        Package = package;
        Variation = variation;
        Dimensions = dimensions;
        CreatedBy = createdBy;
        Notes = notes;
        Research = research;
    }

    [GraphQLType(typeof(IdType))]
    [GraphQLNonNullType]
    public string Id { get; }

    public string? Sku { get; }

    public string? Package { get; }

    public ProductVariation? Variation { get; }

    public ProductDimension? Dimensions { get; }

    [Provides("totalProductsCreated")]
    public User? CreatedBy { get; }

    [ApolloTag("internal")]
    public string? Notes { get; }

    public List<ProductResearch> Research { get; }

    [ReferenceResolver]
    public static Product? GetProductById(
        string id,
        Data repository)
        => repository.Products.FirstOrDefault(t => id.Equals(t.Id));

    [ReferenceResolver]
    public static Product? GetProductByPackage(
        string sku,
        string package,
        Data repository)
        => repository.Products.FirstOrDefault(
            t => sku.Equals(t.Sku) && package.Equals(t.Package));

    [ReferenceResolver]
    public static Product? GetProductByVariation(
        string sku,
        [Map("variation.id")] string variationId,
        Data repository)
        => repository.Products.FirstOrDefault(
            t => sku.Equals(t.Sku) && variationId.Equals(t.Variation?.Id)
        );
}
