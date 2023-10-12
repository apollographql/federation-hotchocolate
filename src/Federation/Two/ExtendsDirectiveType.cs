using ApolloGraphQL.HotChocolate.Federation.Constants;
using ApolloGraphQL.HotChocolate.Federation.Properties;

namespace ApolloGraphQL.HotChocolate.Federation.Two;

/// <summary>
/// <code>
/// directive @extends on OBJECT | INTERFACE
/// </code>
/// 
/// The @extends directive is used to represent type extensions in the schema. Federated extended types should have 
/// corresponding @key directive defined that specifies primary key required to fetch the underlying object.
/// 
/// NOTE: Federation v2 no longer requires `@extends` directive due to the smart entity type merging. All usage of @extends
/// directive should be removed from your Federation v2 schemas.
/// <example>
/// # extended from the Users service
/// type Foo @extends @key(fields: "id") {
///   id: ID
///   description: String
/// }
/// </example>
/// </summary>
[Obsolete("@extends directive is no longer needed for Federation v2 schemas")]
public sealed class ExtendsDirectiveType : DirectiveType
{
    protected override void Configure(IDirectiveTypeDescriptor descriptor)
        => descriptor
            .Name(WellKnownTypeNames.Extends)
            .Description(FederationResources.ExtendsDirective_Description)
            .Location(DirectiveLocation.Object | DirectiveLocation.Interface);
}
