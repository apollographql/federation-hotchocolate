using HotChocolate.Types;

namespace ApolloGraphQL.HotChocolate.Federation.CertificationSchema.CodeFirst.Types;

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
        descriptor.Field(v => v.Id).Type<NonNullType<IdType>>();
    }
}