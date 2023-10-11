namespace ApolloGraphQL.HotChocolate.Federation;

[AttributeUsage(
    AttributeTargets.Class
    | AttributeTargets.Struct
    | AttributeTargets.Method
    | AttributeTargets.Property
)]
public sealed class TagAttribute : Attribute
{

    public TagAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
