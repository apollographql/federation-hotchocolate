using System.Collections.Generic;

namespace ApolloGraphQL.HotChocolate.Federation.Two;

public class FederatedSchema : Schema
{
    public FederatedSchema() : this(new List<Link> { Link.LatestFederationVersionLinkImport() })
    {
    }

    public FederatedSchema(List<Link> links)
    {
        Links = links;
    }

    public List<Link> Links { get; }

    protected override void Configure(ISchemaTypeDescriptor descriptor)
    {

        // this.GetType().IsDefined(typeof)

        foreach (Link link in Links)
        {
            descriptor.Directive(link);
        }
    }
}