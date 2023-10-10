using ApolloGraphQL.HotChocolate.Federation;
using ApolloGraphQL.HotChocolate.Federation.Constants;
using ApolloGraphQL.HotChocolate.Federation.Properties;

namespace ApolloGraphQL.HotChocolate.Federation.Two;

public sealed class InaccessibleDirectiveType : DirectiveType
{
    protected override void Configure(IDirectiveTypeDescriptor descriptor)
        => descriptor
            .Name(WellKnownTypeNames.Inaccessible)
            .Description(FederationResources.InaccessibleDirective_Description)
            .Location(
                DirectiveLocation.FieldDefinition
                | DirectiveLocation.Object
                | DirectiveLocation.Interface
                | DirectiveLocation.Union
                | DirectiveLocation.Enum
                | DirectiveLocation.EnumValue
                | DirectiveLocation.Scalar
                | DirectiveLocation.Scalar
                | DirectiveLocation.InputObject
                | DirectiveLocation.InputFieldDefinition
                | DirectiveLocation.ArgumentDefinition);
}
