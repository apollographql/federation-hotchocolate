using ApolloGraphQL.HotChocolate.Federation.Constants;
using ApolloGraphQL.HotChocolate.Federation.Properties;

namespace ApolloGraphQL.HotChocolate.Federation.Two;

/// <summary>
/// Note: This @override implementation applies to Federation v2.0 through v2.6.
/// As of v2.7, the @override directive includes an additional `label` argument.
/// The implementation for v2.7+ can be found in ProgressiveOverrideDirectiveType.
/// <code>
/// directive @override(from: String!) on FIELD_DEFINITION
/// </code>
/// 
/// The @override directive is used to indicate that the current subgraph is taking
/// responsibility for resolving the marked field away from the subgraph specified in the from
/// argument. Name of the subgraph to be overridden has to match the name of the subgraph that
/// was used to publish their schema.
/// <example>
/// type Foo @key(fields: "id") {
///   id: ID!
///   description: String @override(from: "BarSubgraph")
/// }
/// </example>
/// </summary>
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
