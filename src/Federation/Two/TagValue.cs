namespace ApolloGraphQL.HotChocolate.Federation.Two;

/// <summary>
/// Object representation of a @tag directive.
/// </summary>
public sealed class TagValue
{

    /// <summary>
    /// Initializes  a new instance of <see cref="Tag"/>.
    /// </summary>
    /// <param name="name">
    /// Custom tag value
    /// </param>
    public TagValue(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Get tag value.
    /// </summary>
    public string Name { get; }
}
