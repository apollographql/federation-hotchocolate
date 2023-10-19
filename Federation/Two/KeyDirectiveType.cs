using ApolloGraphQL.HotChocolate.Federation.Constants;
using ApolloGraphQL.HotChocolate.Federation.Properties;

namespace ApolloGraphQL.HotChocolate.Federation.Two;

/// <summary>
/// <code>
/// directive @key(fields: FieldSet!, resolvable: Boolean = true) repeatable on OBJECT | INTERFACE
/// </code>
/// 
/// The @key directive is used to indicate a combination of fields that can be used to uniquely
/// identify and fetch an object or interface. The specified field set can represent single field (e.g. "id"),
/// multiple fields (e.g. "id name") or nested selection sets (e.g. "id user { name }"). Multiple keys can
/// be specified on a target type.
/// 
/// Keys can also be marked as non-resolvable which indicates to router that given entity should never be
/// resolved within given subgraph. This allows your subgraph to still reference target entity without
/// contributing any fields to it.
/// <example>
/// type Foo @key(fields: "id") {
///   id: ID!
///   field: String
///   bars: [Bar!]!
/// }
/// 
/// type Bar @key(fields: "id", resolvable: false) {
///   id: ID!
/// }
/// </example>
/// </summary>
public sealed class KeyDirectiveType : DirectiveType
{
    protected override void Configure(IDirectiveTypeDescriptor descriptor)
        => descriptor
            .Name(WellKnownTypeNames.Key)
            .Description(FederationResources.KeyDirective_Description)
            .Location(DirectiveLocation.Object | DirectiveLocation.Interface)
            .Repeatable()
            .FieldsArgumentV2()
            .Argument(WellKnownArgumentNames.Resolvable)
            .Type<BooleanType>()
            .DefaultValue(true);
}
