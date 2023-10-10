namespace ApolloGraphQL.HotChocolate.Federation;

public sealed class TagAttribute : Attribute
{

    public TagAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
