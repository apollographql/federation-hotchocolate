using ApolloGraphQL.HotChocolate.Federation;
using ApolloGraphQL.HotChocolate.Federation.Constants;
using ApolloGraphQL.HotChocolate.Federation.Properties;

namespace ApolloGraphQL.HotChocolate.Federation.Two;

public sealed class ShareableDirectiveType : DirectiveType
{
    protected override void Configure(IDirectiveTypeDescriptor descriptor)
        => descriptor
            .Name(WellKnownTypeNames.Shareable)
            .Description(FederationResources.ShareableDirective_Description)
            .Location(DirectiveLocation.FieldDefinition | DirectiveLocation.Object)
            .Repeatable();
}
