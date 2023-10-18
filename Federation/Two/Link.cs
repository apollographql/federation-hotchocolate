using System.Collections.Generic;

namespace ApolloGraphQL.HotChocolate.Federation.Two;

/// <summary>
/// Object representation of @link directive.
/// </summary>
public sealed class Link
{
    /// <summary>
    /// Initializes new instance of <see cref="Link"/> 
    /// </summary>
    /// <param name="url">
    /// Url of specification to be imported
    /// </param>
    /// <param name="import">
    /// Optional list of imported elements.
    /// </param>
    public Link(string url, List<string?>? import)
    {
        Url = url;
        Import = import;
    }

    /// <summary>
    /// Gets imported specification url.
    /// </summary>
    public string Url { get; }

    /// <summary>
    /// Gets optional list of imported element names.
    /// </summary>

    public List<string?>? Import { get; }
}
