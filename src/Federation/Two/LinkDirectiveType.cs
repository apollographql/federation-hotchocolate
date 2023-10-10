using ApolloGraphQL.HotChocolate.Federation.Constants;
using DirectiveLocationType = HotChocolate.Types.DirectiveLocation;

namespace ApolloGraphQL.HotChocolate.Federation.Two;

public sealed class LinkDirectiveType : DirectiveType<Link>
{
    protected override void Configure(IDirectiveTypeDescriptor<Link> descriptor)
        => descriptor
            .Name(WellKnownTypeNames.Link)
            .Location(DirectiveLocationType.Schema)
            .Repeatable();
}
