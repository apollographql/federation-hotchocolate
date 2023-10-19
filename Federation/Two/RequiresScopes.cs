using System.Collections.Generic;

namespace ApolloGraphQL.HotChocolate.Federation.Two;

/// <summary>
/// Object representation of @requiresScopes directive.
/// </summary>
public sealed class RequiresScopes
{
    /// <summary>
    /// Initializes new instance of <see cref="RequiresScopes"/> 
    /// </summary>
    /// <param name="scopes">
    /// List of a list of required JWT scopes.
    /// </param>
    public RequiresScopes(List<List<Scope>> scopes)
    {
        Scopes = scopes;
    }

    /// <summary>
    /// Retrieves list of a list of required JWT scopes.
    /// </summary>
    public List<List<Scope>> Scopes { get; }
}
