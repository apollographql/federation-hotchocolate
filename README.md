# This package is now officially deprecated in favor of the HotChocolate built-in Federation module. 
## We created this package when there was no built-in HC support for Federation v2, but as of HC v13.9.0 `HotChocolate.ApolloFederation` package now fully supports Federation v2. In order to simplify the integrations in the HC ecosystem, we decided to deprecate this package in favor of supporting a single fully featured built-in package.

[![Continuous Integration](https://github.com/apollographql/federation-hotchocolate/workflows/Continuous%20Integration/badge.svg)](https://github.com/apollographql/federation-hotchocolate/actions?query=workflow%3A%22Continuous+Integration%22)
[![MIT License](https://img.shields.io/github/license/apollographql/federation-hotchocolate.svg)](LICENSE)
[![Nuget](https://img.shields.io/nuget/v/ApolloGraphQL.HotChocolate.Federation)](https://www.nuget.org/packages/ApolloGraphQL.HotChocolate.Federation/)
[![Join the community forum](https://img.shields.io/badge/join%20the%20community-forum-blueviolet)](https://community.apollographql.com)
[![Join our Discord server](https://img.shields.io/discord/1022972389463687228.svg?color=7389D8&labelColor=6A7EC2&logo=discord&logoColor=ffffff&style=flat-square)](https://discord.gg/graphos)

# Apollo Federation for Hot Chocolate

> **Warning**
> Due to a breaking change to the public API, we cannot support newer versions of `HotChocolate` until their replacement API (currently work in progress) is complete. **We can only support v`13.5.x` and v`13.6.x` releases**.

[**Apollo Federation**](https://www.apollographql.com/docs/federation/) is a powerful, open architecture that helps you create a **unified supergraph** that combines multiple GraphQL APIs.
`ApolloGraphQL.HotChocolate.Federation` provides Apollo Federation support for building subgraphs in the `HotChocolate` ecosystem. Individual subgraphs can be run independently of each other but can also specify
relationships to the other subgraphs by using Federated directives. See [Apollo Federation documentation](https://www.apollographql.com/docs/federation/) for details.

## Installation

`ApolloGraphQL.HotChocolate.Federation` package is published to [Nuget](https://www.nuget.org/packages/ApolloGraphQL.HotChocolate.Federation). Update your `.csproj` file with following package references

```xml
  <ItemGroup>
    <!-- make sure to also include HotChocolate package -->
    <PackageReference Include="HotChocolate.AspNetCore" Version="13.6.0" />
    <!-- federation package -->
    <PackageReference Include="ApolloGraphQL.HotChocolate.Federation" Version="$LatestVersion" />
  </ItemGroup>
```

After installing the necessary packages, you need to register Apollo Federation with your GraphQL service.


```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddApolloFederationV2() 
    // register your types and services
    ;

var app = builder.Build();
app.MapGraphQL();
app.Run();
```

> If you would like to opt-in to Federation v1 schema, you need to use `.AddApolloFederation()` extension instead.

## Usage

Refer to [`HotChocolate` documentation](https://chillicream.com/docs/hotchocolate/v13) for detailed information on how to create GraphQL schemas and configure your server.

Apollo Federation requires subgraphs to provide some additional metadata to make them supergraph aware. Entities are GraphQL objects that can be uniquely identified across 
the supergraph by the specified `@key`s. Since entities can be extended by various subgraphs, we need an extra entry point to access the entities, i.e. subgraphs need to
implement reference resolvers for entities that they support.

See [Apollo documentation](https://www.apollographql.com/docs/federation/) for additional Federation details.

### Annotation

All federated directives are provided as attributes that can be applied directly on classes/fields/methods.

```csharp
[Key("id")]
public class Product
{
    public Product(string id, string name, string? description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    [ID]
    public string Id { get; }

    public string Name { get; }

    public string? Description { get; }

    // assumes ProductRepository with GetById method exists
    // reference resolver method must be public static
    [ReferenceResolver]
    public static Product GetByIdAsync(
        string id,
        ProductRepository productRepository)
        => productRepository.GetById(id);
}
```

This will generate following type

```graphql
type Product @key(fields: "id") {
    id: ID!
    name: String!
    description: String
}
```

#### Federation Attributes

Federation v1 directives

* `Extends` applicable on objects, see [`@extends` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#extends)
* `External` applicable on fields, see [`@external` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#external)
* `Key` applicable on objects, see [`@key` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#key)
* `Provides` applicable on fields, see [`@provides` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#provides)
* `Requires` applicable on fields, see [`@requires` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#requires)

Federation v2 directives (includes all of the v1 directives)

* `ApolloTag` applicable on schema, see [`@tag` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#tag)
* `ApolloAuthenticated` **(since v2.5)** applicable on enum, field, interface and object, [`@authenticated` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives/#authenticated)
* `ComposeDirective` **(since v2.1)** applicable on schema, see [`@composeDirective` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#composedirective)
* `Contact` applicable on schema, see [`@contact` usage](#providing-subgraph-contact-information)
* `Inaccessible` applicable on all type definitions, see [`@inaccessible` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#inaccessible)
* `InterfaceObject` **(since v2.3)** applicable on objects, see [`@interfaceObject` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#interfaceobject)
* `KeyInterface` applicable on interfaces, see [entity interface `@key` documentation](https://www.apollographql.com/docs/federation/federated-types/interfaces)
* `Link` applicable on schema, see [`@link` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#the-link-directive)
* `RequiresScopes` **(since v2.5)** applicable on enum, field, interface and object, [`@requiresScopes` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives/#requiresscopes)
* `Shareable` applicable on schema, see [`@shareable` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#shareable)

Entity resolution

* `Map` applicable on entity resolver method paramaters, allows you to map complex argument to a simpler representation value, e.g. `[Map("foo.bar")] string bar`
* `ReferenceResolver` applicable on **public static methods** within an entity class to indicate entity resolver

### Code First

Alternatively, if you need more granular control, you can use code first approach and manually populate federation information on the underlying GraphQL type
descriptor. All federated directives expose corresponding methods on the applicable descriptor.

```csharp
public class Product
{
    public Product(string id, string name, string? description)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    [ID]
    public string Id { get; }

    public string Name { get; }

    public string? Description { get; }
}

public class ProductType : ObjectType<Product>
{
    protected override void Configure(IObjectTypeDescriptor<Product> descriptor)
    {
        descriptor
            .Key("id")
            .ResolveReferenceWith(t => GetProduct(default!, default!));
    }

    private static Product GetProduct(
        string id,
        ProductRepository productRepository)
        => productRepository.GetById(upc);
}
```

This will generate following type

```graphql
type Product @key(fields: "id") {
    id: ID!
    name: String!
    description: String
}
```

#### Descriptor Extensions

Federation v1 directives

* `ExtendsType` applicable on objects, see [`@extends` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#extends)
* `External` applicable on fields, see [`@external` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#external)
* `Key(fieldset)` applicable on objects, see [`@key` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#key)
* `Provides(fieldset)` applicable on fields, see [`@provides` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#provides)
* `Requires(fieldset)` applicable on fields, see [`@requires` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#requires)

Federation v2 directives (includes all of the v1 directives)

* `ApolloTag` applicable on all type definitions, see [`@tag` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#tag)
* `ApolloAuthenticated` **(since v2.5)** applicable on enum, field, interface and object, [`@authenticated` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives/#authenticated)
* `ComposeDirective(name)` **(since v2.1)** applicable on schema, see [`@composeDirective` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#composedirective)
* `Contact(name, url?, description?)` applicable on schema, see [`@contact` usage](#providing-subgraph-contact-information)
* `Inaccessible` applicable on all type definitions, see [`@inaccessible` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#inaccessible)
* `InterfaceObject` **(since v2.3)** applicable on objects, see [`@interfaceObject` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#interfaceobject)
* `Key(fieldset)` applicable on objects, see [`@key` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#key)
* `Link(url, [import]?)` applicable on schema, see [`@link` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#the-link-directive)
* `NonResolvableKey(fieldset)` applicable on objects, see [non-resolvable `@key` documentation](https://www.apollographql.com/docs/federation/entities#referencing-an-entity-without-contributing-fields)
* `RequiresScopes(scopes)` **(since v2.5)** applicable on enum, field, interface and object, [`@requiresScopes` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives/#requiresscopes)
* `Shareable` applicable on fields and objects, see [`@shareable` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#shareable)

Entity resolution

* you have to provide `ResolveReferenceWith` function to be able to resolve the entities

### Advanced Use Cases

#### Generating schema at build time

See [HotChocolate documentation](https://chillicream.com/docs/hotchocolate/v13/server/command-line) for details on the server support for command line interface.
In order to generate schema at build time, you need to add additional dependency on `HotChocolate.AspNetCore.CommandLine` package and configure your server to allow it
to `RunWithGraphQLCommands`.

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddApolloFederationV2()
    // register your types and services
    ;

var app = builder.Build();
app.MapGraphQL();
app.RunWithGraphQLCommands(args);
```

You can then generate your schema by running

```shell
dotnet run -- schema export --output schema.graphql
```

#### Specifying Federation Version

By default, `ApolloGraphQL.HotChocolate.Federation` will generate schema using latest supported Federation version. If you would like to opt-in to use older versions you can
so by specifying the version when configuring `AddApolloFederationV2` extension.

```csharp
builder.Services
    .AddGraphQLServer()
    .AddApolloFederationV2(FederationVersion.FEDERATION_23)
    // register your types and services
    ;
```

Alternatively, you can also provide custom `FederatedSchema` that targets specific Federation version

```csharp
public class CustomSchema : FederatedSchema
{
    public CustomSchema() : base(FederationVersion.FEDERATION_23) {
    }
}

builder.Services
    .AddGraphQLServer()
    .AddApolloFederationV2(new CustomSchema())
    // register your types and services
    ;
```

### Customizing Schema

If you would like to customize your schema by applying some directives, you can also provide custom `FederatedSchema` that can be annotated with attributes extending `SchemaTypeDescriptorAttribute`

```csharp
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = true)]
public sealed class CustomAttribute : SchemaTypeDescriptorAttribute
{
    public override void OnConfigure(IDescriptorContext context, ISchemaTypeDescriptor descriptor, Type type)
    {
        // configure your directive here
    }
}

[Custom]
public class CustomSchema : FederatedSchema
{
    public CustomSchema() : base(FederationVersion.FEDERATION_23) {
    }
}

builder.Services
    .AddGraphQLServer()
    .AddApolloFederationV2(new CustomSchema())
    // register your types and services
    ;
```

Alternatively, you can also specify custom schema configuration action when building federated subgraph

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddApolloFederationV2(schemaConfiguration: s =>
    {
        // apply your directive here
    })
    // register your types and services
    ;
```

#### Non-resolvable `@key`

Your subgraphs can use an entity as a field's return type without contributing any fields to that entity. Since we still need a type definition to generate a valid schema,
we can define a **stub** object with `[NonResolvableKeyAttribute]`.

```csharp
public class Review {
    public Review(Product product, int score)
    {
        Product = product;
        Score = score
    }

    public Product Product { get; }
    public int Score { get; }
}


[NonResolvableKey("id")]
public class Product {
    public Product(string id)
    {
        Id = id;
    }

    public string Id { get; }
}
```

#### `@composedDirective` usage

By default, Supergraph schema excludes all custom directives. The `@composeDirective` is used to specify custom directives that should be preserved in the Supergraph schema.

`ApolloGraphQL.HotChocolate.Federation` provides common `FederatedSchema` class that automatically applies Apollo Federation v2 `@link` definition. When applying any custom
schema directives, you should extend this class and add required attributes/directives.

When applying `@composedDirective` you also need to `@link` it your specification. Your custom schema should then be passed to the `AddApolloFederationV2` extension.

```csharp
[ComposeDirective("@custom")]
[Link("https://myspecs.dev/myCustomDirective/v1.0", new string[] { "@custom" })]
public class CustomSchema : FederatedSchema
{
}

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddApolloFederationV2(new CustomSchema())
    // register your types and services
    ;
```

Alternatively, you can apply `@composedDirective` by directly applying it on a target schema by using configuration action

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddApolloFederationV2(schemaConfiguration: s =>
    {
        s.Link("https://myspecs.dev/myCustomDirective/v1.0", new string[] { "@custom" });
        s.ComposeDirective("@custom");
    })
    // register your types and services
    ;
```

#### `@interfaceObject` usage

Apollo Federation v2 supports **entity interfaces**, a powerful extension to the GraphQL interfaces that allows you to extend functionality of an interface across the supergraph
without having to implement (or even be aware of) all its implementing types.

In a subgraph defininig the interface we need to apply `@key`

```csharp
[InterfaceType]
[KeyInterface("id")]
public interface Product
{
    [ID]
    string Id { get; }

    string Name { get; }
}

[Key("id")]
public class Book : Product
{
    [ID]
    public string Id { get; set; }

    public string Name { get; set; }

    public string Content { get; set; }
}
```

We can then extend the interface in another subgraph by making it a type, applying `@interfaceObject` and same `@key` directive. This allows you add new fields to every
entity that implements your interface (e.g. adding `Reviews` field to all `Product` implementations).

```csharp
[Key("id")]
[InterfaceObject]
public class Product
{
    [ID]
    public string Id { get; set; }

    public List<string> Reviews { get; set; }
}
```

#### Access control through @requiresScopes

The `@requiresScopes` directive is used to indicate that the target element is accessible only to the authenticated supergraph users with the appropriate JWT scopes. 
Refer to the [Apollo Router article](https://www.apollographql.com/docs/router/configuration/authorization#requiresscopes) for additional details.

```csharp
public class Query
{
    [RequiresScopes(scopes: new string[] { "scope1, scope2", "scope3" })]
    [RequiresScopes(scopes: new string[] { "scope4" })]
    public Product? GetProduct([ID] string id, Data repository)
        => repository.Products.FirstOrDefault(t => t.Id.Equals(id));
}
```

This will generate the following schema

```graphql
type Query {
    product(id: ID!): Product @requiresScopes(scopes: [ [ "scope1, scope2", "scope3" ], [ "scope4" ] ])
}
```

#### Providing subgraph contact information

You can use the `@contact` directive to add your team's contact information to a subgraph schema. This information is displayed in Studio, which helps *other* teams know who
to contact for assistance with the subgraph. See [documentation](https://www.apollographql.com/docs/graphos/graphs/federated-graphs/#contact-info-for-subgraphs) for details.

We need to apply `[Contact]` attribute on a schema. You can either apply `[Contact]` attribute on a custom schema and pass your custom schema to the `AddApolloFederationV2` extension.

```csharp
[Contact("MyTeamName", "https://myteam.slack.com/archives/teams-chat-room-url", "send urgent issues to [#oncall](https://yourteam.slack.com/archives/oncall)")]
public class CustomSchema : FederatedSchema
{
}

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddType<ContactDirectiveType>();
    .AddApolloFederationV2(new CustomSchema())
    // register your types and services
    ;
```

or apply `@contact` directive directly on a schema by providing custom schema configuration action

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddApolloFederationV2(schemaConfiguration: s =>
    {
        s.Contact("MyTeamName", "https://myteam.slack.com/archives/teams-chat-room-url", "send urgent issues to [#oncall](https://yourteam.slack.com/archives/oncall)");
    })
    // register your types and services
    ;
```

#### Custom Query types

`ApolloGraphQL.HotChocolate.Federation` automatically defaults to use `Query` type name. When using custom root `Query` operation types, you have to explicitly configure schema
with those custom values.

```csharp
public class CustomQuery
{
    public Foo? GetFoo([ID] string id, Data repository)
        => repository.Foos.FirstOrDefault(t => t.Id.Equals(id));
}

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddGraphQLServer()
    .ModifyOptions(opts => opts.QueryTypeName = "CustomQuery")
    .AddApolloFederationV2()
    .AddQueryType<CustomQuery>()
    // register your other types and services
    ;

var app = builder.Build();
app.MapGraphQL();
app.Run();
```

### Migration Guide

Migrating from `HotChocolate.Federation` to `ApolloGraphQL.HotChocolate.Federation` is easy. Simply update your package import to point to a new module

```diff
  <ItemGroup>
    <!-- make sure to also include HotChocolate package -->
    <PackageReference Include="HotChocolate.AspNetCore" Version="13.6.0" />
    <!-- federation package -->
-    <PackageReference Include="HotChocolate.ApolloFederation" Version="$LatestVersion" />
+    <PackageReference Include="ApolloGraphQL.HotChocolate.Federation" Version="$LatestVersion" />
  </ItemGroup>
```

and update namespace imports

```diff
- using HotChocolate.ApolloFederation;
+ using ApolloGraphQL.HotChocolate.Federation;
```

While we tried to make migration process as seamless as possible, we had to make few tweaks to the library. Due to the dependency on some of the internal APIs, we had to make following breaking changes to the library:

* `[Key]` is now applicable **only on classes** and you no longer can apply it on individual fields
* `[ReferenceResolver]` is now applicable **only on public static methods within an entity**, it is no longer applicable on classes

## Known Limitations

#### Entity Resolver Auto-Map Only Scalar Values

`[EntityResolver]`s can automatically map entity representation to the supported `@key`/`@requires` values. Scalars `@key` fields are automatically mapped and we can use `[Map]` attribute to auto map scalar values from complex selection sets.

Currently we don't support auto-mapping of [List](https://github.com/apollographql/federation-hotchocolate/issues/19) and [Object](https://github.com/apollographql/federation-hotchocolate/issues/20) values.

As a workaround, you need to manually parse the representation object in your implementation.

```csharp
[ReferenceResolver]
public static Foo GetByFooBar(
    [LocalState] ObjectValueNode data
    Data repository)
{
    // TODO implement logic here by manually reading values from local state data
}
```

#### Limited `@link` support

Currently we only support importing elements from the referenced subgraphs.

Namespacing and renaming elements is currently unsupported. See [issue](https://github.com/apollographql/federation-hotchocolate/issues/24) for details.

## Contact

If you have a specific question about the library or code, please start a discussion in the [Apollo community forums](https://community.apollographql.com/) or start a conversation on our [Discord server](https://discord.gg/graphos).

## Contributing

To get started, please fork the repo and checkout a new branch. You can then build the library locally by running

```shell
# install dependencies
dotnet restore
# build project
dotnet build
# run tests
dotnet test
```

See more info in [CONTRIBUTING.md](CONTRIBUTING.md).

After you have your local branch set up, take a look at our open issues to see where you can contribute.

## Security

For more info on how to contact the team for security issues, see our [Security Policy](https://github.com/apollographql/federation-hotchocolate/security/policy).

## License

This library is licensed under [The MIT License (MIT)](LICENSE).
