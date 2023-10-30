using ApolloGraphQL.HotChocolate.Federation.Constants;
using ApolloGraphQL.HotChocolate.Federation.Properties;

namespace ApolloGraphQL.HotChocolate.Federation.Two;

/// <summary>
/// <code>
/// directive @tag(name: String!) repeatable on FIELD_DEFINITION 
///  | OBJECT
///  | INTERFACE
///  | UNION
///  | ENUM
///  | ENUM_VALUE
///  | SCALAR
///  | INPUT_OBJECT
///  | INPUT_FIELD_DEFINITION
///  | ARGUMENT_DEFINITION
/// </code>
/// 
/// The @tag directive allows users to annotate fields and types with additional metadata information.
/// Tagging is commonly used for creating variants of the supergraph using contracts.
/// <example>
/// type Foo @tag(name: "internal") {
///   id: ID!
///   name: String
/// }
/// </example>
/// </summary>
public sealed class TagDirectiveType : DirectiveType<TagValue>
{
    protected override void Configure(IDirectiveTypeDescriptor<TagValue> descriptor)
        => descriptor
            .BindArgumentsImplicitly()
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
                | DirectiveLocation.ArgumentDefinition);
}
