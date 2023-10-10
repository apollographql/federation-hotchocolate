using ApolloGraphQL.HotChocolate.Federation.Constants;
using ApolloGraphQL.HotChocolate.Federation.Properties;

namespace ApolloGraphQL.HotChocolate.Federation.Two;

/*
directive @composeDirective(name: String!) repeatable on SCHEMA
*/
public sealed class ComposeDirectiveType : DirectiveType
{
    protected override void Configure(IDirectiveTypeDescriptor descriptor)
        => descriptor
            .Name(WellKnownTypeNames.ComposeDirective)
            .Description(FederationResources.ComposeDirective_Description)
            .Location(DirectiveLocation.Schema)
            .Argument(WellKnownArgumentNames.Name)
            .Type<NonNullType<StringType>>();
}
