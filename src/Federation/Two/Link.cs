using System.Collections.Generic;

namespace ApolloGraphQL.HotChocolate.Federation.Two;

public sealed class Link
{

    public Link(string url, List<string?>? import)
    {
        Url = url;
        Import = import;
    }

    public string Url { get; }

    public List<string?>? Import { get; }
}
