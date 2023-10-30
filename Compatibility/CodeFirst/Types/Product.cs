using ApolloGraphQL.HotChocolate.Federation;

namespace Products;

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

    public string Id { get; }

    public string? Sku { get; }

    public string? Package { get; }

    public ProductVariation? Variation { get; }

    public ProductDimension? Dimensions { get; }

    public User? CreatedBy { get; }

    public string? Notes { get; }

    public List<ProductResearch> Research { get; }
}

public class ProductType : ObjectType<Product>
{
    protected override void Configure(IObjectTypeDescriptor<Product> descriptor)
    {
        descriptor.BindFieldsImplicitly();
        descriptor.Field(p => p.Id).Type<NonNullType<IdType>>();
        descriptor.Field(p => p.Variation).Type<ProductVariationType>();
        descriptor.Field(p => p.Dimensions).Type<ProductDimensionType>();
        descriptor.Field(p => p.CreatedBy)
            .Type<UserType>()
            .Provides("totalProductsCreated");
        descriptor.Field(p => p.Notes).ApolloTag("internal");
        descriptor.Field(p => p.Research).Type<NonNullType<ListType<NonNullType<ProductResearchType>>>>();
        descriptor.Directive(CustomDirectiveType.CustomDirectiveName);
        descriptor.Key("id")
            .ResolveReferenceWith(p => GetProductById(default!, default!));
        descriptor.Key("sku package")
            .ResolveReferenceWith(p => GetProductByPackage(default!, default!, default!));
        descriptor.Key("sku variation { id }")
            .ResolveReferenceWith(p => GetProductByVariation(default!, default!, default!));
    }

    private static Product? GetProductById(
        string id,
        Data repository)
        => repository.Products.FirstOrDefault(t => id.Equals(t.Id));

    private static Product? GetProductByPackage(
        string sku,
        string package,
        Data repository)
        => repository.Products.FirstOrDefault(
            t => sku.Equals(t.Sku) && package.Equals(t.Package));

    // TODO [Map]
    private static Product? GetProductByVariation(
        string sku,
        [Map("variation.id")] string variationId,
        Data repository)
        => repository.Products.FirstOrDefault(
            t => sku.Equals(t.Sku) && variationId.Equals(t.Variation?.Id)
        );
}
