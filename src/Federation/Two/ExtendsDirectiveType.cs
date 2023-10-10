using ApolloGraphQL.HotChocolate.Federation.Constants;
using ApolloGraphQL.HotChocolate.Federation.Properties;

namespace ApolloGraphQL.HotChocolate.Federation.Two;

public sealed class ExtendsDirectiveType : DirectiveType
{
    protected override void Configure(IDirectiveTypeDescriptor descriptor)
        => descriptor
            .Name(WellKnownTypeNames.Extends)
            .Description(FederationResources.ExtendsDirective_Description)
            .Location(DirectiveLocation.Object | DirectiveLocation.Interface);
}
