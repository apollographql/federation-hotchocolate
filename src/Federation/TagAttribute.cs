namespace ApolloGraphQL.HotChocolate.Federation;

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
/// 
/// NOTE: Only available in Federation v2
/// <example>
/// type Foo @tag(name: "internal") {
///   id: ID!
///   name: String
/// }
/// </example>
/// </summary>
[AttributeUsage(
    AttributeTargets.Class
    | AttributeTargets.Struct
    | AttributeTargets.Method
    | AttributeTargets.Property
)]
public sealed class TagAttribute : Attribute
{

    public TagAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
