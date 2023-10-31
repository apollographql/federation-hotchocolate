namespace Products;

public class User
{
    public User(string email, string? name)
    {
        Email = email;
        Name = name;
    }

    public int? GetAverageProductsCreatedPerYear()
    {
        if (TotalProductsCreated != null && LengthOfEmployment != null)
        {
            return Convert.ToInt32((TotalProductsCreated * 1.0) / LengthOfEmployment);
        }
        return null;
    }

    public string Email { get; set; }

    public string? Name { get; }

    public int? TotalProductsCreated { get; set; } = 1337;

    public int? LengthOfEmployment { get; set; }

    public int GetYearsOfEmployment()
    {
        if (LengthOfEmployment == null)
        {
            throw new InvalidOperationException("yearsOfEmployment should never be null - it should be populated by _entities query");
        }

        return (int)LengthOfEmployment;
    }
}

public class UserType : ObjectType<User>
{
    protected override void Configure(IObjectTypeDescriptor<User> descriptor)
    {
        descriptor.BindFieldsImplicitly();
        descriptor.ExtendsType();
        descriptor.Field(u => u.GetAverageProductsCreatedPerYear())
            .Requires("totalProductsCreated yearsOfEmployment");
        descriptor.Field(u => u.Email)
            .Type<NonNullType<IdType>>()
            .External();
        descriptor.Field(u => u.Name)
            .Override("users");
        descriptor.Field(u => u.TotalProductsCreated)
            .External();
        descriptor.Field(u => u.GetYearsOfEmployment())
            .External();
        descriptor.Ignore(u => u.LengthOfEmployment);
        descriptor.Key("email")
            .ResolveReferenceWith(u => GetUserByEmail(default!, default, default, default!));
    }

    private static User? GetUserByEmail(
        string email,
        int? totalProductsCreated,
        int? yearsOfEmployment,
        Data repository)
    {
        User? user = repository.Users.FirstOrDefault(u => u.Email.Equals(email));
        if (user != null)
        {
            if (totalProductsCreated != null)
            {
                user.TotalProductsCreated = totalProductsCreated;
            }

            if (yearsOfEmployment != null)
            {
                user.LengthOfEmployment = yearsOfEmployment;
            }
        }
        return user;
    }
}
