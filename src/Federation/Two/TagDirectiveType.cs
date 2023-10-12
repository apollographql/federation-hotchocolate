using ApolloGraphQL.HotChocolate.Federation.Constants;
using ApolloGraphQL.HotChocolate.Federation.Properties;

namespace ApolloGraphQL.HotChocolate.Federation.Two;

/*
directive @tag(name: String!) repeatable on
  | FIELD_DEFINITION
  | INTERFACE
  | OBJECT
  | UNION
  | ARGUMENT_DEFINITION
  | SCALAR
  | ENUM
  | ENUM_VALUE
  | INPUT_OBJECT
  | INPUT_FIELD_DEFINITION
*/
public sealed class TagDirectiveType : DirectiveType
{
    protected override void Configure(IDirectiveTypeDescriptor descriptor)
        => descriptor
            .Name(WellKnownTypeNames.Tag)
            .Description(FederationResources.TagDirective_Description)
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
                | DirectiveLocation.ArgumentDefinition)
            .Argument(WellKnownArgumentNames.Name)
            .Type<NonNullType<StringType>>();
}
