using ApolloGraphQL.HotChocolate.Federation.Constants;
using ApolloGraphQL.HotChocolate.Federation.Properties;

namespace ApolloGraphQL.HotChocolate.Federation.Two;

/// <summary>
/// <code>
/// directive @override(from: String!, label: String) on FIELD_DEFINITION
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
public sealed class ProgressiveOverrideDirectiveType : DirectiveType
{
    protected override void Configure(IDirectiveTypeDescriptor descriptor)
    { 
        IDirectiveTypeDescriptor directive = descriptor
            .Name(WellKnownTypeNames.Override)
            .Description(FederationResources.OverrideDirective_Description)
            .Location(DirectiveLocation.FieldDefinition);
        
        directive
            .Argument(WellKnownArgumentNames.From)
            .Type<NonNullType<StringType>>();

        directive
            .Argument(WellKnownArgumentNames.Label)
            .Type<StringType>();
    }
            
}
