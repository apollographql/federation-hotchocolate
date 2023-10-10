using ApolloGraphQL.HotChocolate.Federation;
using ApolloGraphQL.HotChocolate.Federation.Constants;
using ApolloGraphQL.HotChocolate.Federation.Properties;

namespace ApolloGraphQL.HotChocolate.Federation.Two;

public sealed class OverrideDirectiveType : DirectiveType
{
    protected override void Configure(IDirectiveTypeDescriptor descriptor)
        => descriptor
            .Name(WellKnownTypeNames.Override)
            .Description(FederationResources.OverrideDirective_Description)
            .Location(DirectiveLocation.FieldDefinition)
            .Argument(WellKnownArgumentNames.From)
            .Type<NonNullType<StringType>>();
}
