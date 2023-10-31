using HotChocolate;
using HotChocolate.Types;

namespace ApolloGraphQL.HotChocolate.Federation.CertificationSchema.AnnotationBased.Types;

[Key("email")]
[ExtendServiceType]
public class User
{
    public User()
    {
    }

    public User(string email, int? totalProductsCreated)
    {
        Email = email;
        TotalProductsCreated = totalProductsCreated;
    }

    [GraphQLType(typeof(IdType))]
    [GraphQLNonNullType]
    [External]
    public string Email { get; set; } = default!;

    [External]
    public int? TotalProductsCreated { get; set; }
}
