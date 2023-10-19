using HotChocolate.Types;

namespace ApolloGraphQL.HotChocolate.Federation.CertificationSchema.CodeFirst.Types;

public class UserType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor
            .ExtendServiceType()
            .Key("email")
            .ResolveReferenceWith(t => GetUserById(default!));

        descriptor
            .Field(t => t.Email)
            .External()
            .ID();

        descriptor
            .Field(t => t.TotalProductsCreated)
            .External();
    }

    private static User GetUserById(string email) => new(email);
}
