namespace ApolloGraphQL.HotChocolate.Federation;

/// <summary>
/// Maps an argument to a representation value.
/// 
/// WARNING: Only scalar leaf values are supported. Path cannot point to an object nor a list field.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter)]
public class MapAttribute : Attribute
{
    /// <summary>
    /// Represents an argument to a representation value.
    /// </summary>
    /// <param name="path">
    /// A path to field in the representation graph.
    /// </param>
    public MapAttribute(string path)
    {
        Path = path ?? throw new ArgumentNullException(nameof(path));
    }

    /// <summary>
    /// A path to field in the representation graph.
    /// </summary>
    public string Path { get; }
}
