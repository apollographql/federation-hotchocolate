using ApolloGraphQL.HotChocolate.Federation.Constants;
using ApolloGraphQL.HotChocolate.Federation.Properties;

namespace ApolloGraphQL.HotChocolate.Federation.Two;

/// <summary>
/// <code>
/// directive @@requiresScopes(scopes: [[Scope!]!]!) on
///     ENUM
///   | FIELD_DEFINITION
///   | INTERFACE
///   | OBJECT
///   | SCALAR
/// </code>
/// 
/// Directive that is used to indicate that the target element is accessible only to the authenticated supergraph users with the appropriate JWT scopes. 
/// Refer to the <see href = "https://www.apollographql.com/docs/router/configuration/authorization#requiresscopes"> Apollo Router article</see> for additional details.
/// <example>
/// type Foo @key(fields: "id") {
///   id: ID
///   description: String @requiresScopes(scopes: [["scope1"]])
/// }
/// </example>
/// </summary>
public sealed class RequiresScopesDirectiveType : DirectiveType<RequiresScopes>
{
    protected override void Configure(IDirectiveTypeDescriptor<RequiresScopes> descriptor)
        => descriptor
            .BindArgumentsImplicitly()
            .Name(WellKnownTypeNames.RequiresScopes)
            .Description(FederationResources.RequiresScopesDirective_Description)
            .Location(DirectiveLocation.Enum | DirectiveLocation.FieldDefinition | DirectiveLocation.Interface | DirectiveLocation.Object | DirectiveLocation.Scalar);
}
