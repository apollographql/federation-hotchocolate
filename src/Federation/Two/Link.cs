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

    public static Link LatestFederationVersionLinkImport()
    {
        return new Link(
            "https://specs.apollo.dev/federation/v2.5",
            new List<string?> {
                "@composeDirective",
                "@extends",
                "@external",
                "@key",
                "@inaccessible",
                "@interfaceObject",
                "@override",
                "@provides",
                "@requires",
                "@shareable",
                "@tag",
                "FieldSet"
            }
        );
    }
}
