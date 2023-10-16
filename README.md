[![Continuous Integration](https://github.com/apollographql/federation-hotchocolate/workflows/Continuous%20Integration/badge.svg)](https://github.com/apollographql/federation-hotchocolate/actions?query=workflow%3A%22Continuous+Integration%22)
[![MIT License](https://img.shields.io/github/license/apollographql/federation-hotchocolate.svg)](LICENSE)
[![Nuget](https://img.shields.io/nuget/v/ApolloGraphQL.HotChocolate.Federation)](https://www.nuget.org/packages/ApolloGraphQL.HotChocolate.Federation/)
[![Join the community forum](https://img.shields.io/badge/join%20the%20community-forum-blueviolet)](https://community.apollographql.com)
[![Join our Discord server](https://img.shields.io/discord/1022972389463687228.svg?color=7389D8&labelColor=6A7EC2&logo=discord&logoColor=ffffff&style=flat-square)](https://discord.gg/graphos)


# Apollo Federation for Hot Chocolate

>This is a fork of `HotChocolate.Federation` module that aims to provide first class [Apollo Federation](https://www.apollographql.com/docs/federation/) support for [`HotChocolate` subgraphs](https://chillicream.com/docs/hotchocolate/v13).

[**Apollo Federation**](https://www.apollographql.com/docs/federation/) is a powerful, open architecture that helps you create a **unified supergraph** that combines multiple GraphQL APIs.
`ApolloGraphQL.HotChocolate.Federation` provides Apollo Federation support for building subgraphs in the `HotChocolate` ecosystem. Individual subgraphs can be run independently of each other but can also specify
relationships to the other subgraphs by using Federated directives. See [Apollo Federation documentation](https://www.apollographql.com/docs/federation/) for details.

## Installation

`ApolloGraphQL.HotChocolate.Federation` package is published to [Nuget](https://www.nuget.org/packages/ApolloGraphQL.HotChocolate.Federation). Update your `.csproj` file with following package references

```xml
  <ItemGroup>
    <!-- make sure to also include HotChocolate package -->
    <PackageReference Include="HotChocolate.AspNetCore" Version="13.5.1" />
    <!-- federation package -->
    <PackageReference Include="ApolloGraphQL.HotChocolate.Federation" Version="$LatestVersion" />
  </ItemGroup>
```

After installing the necessary packages, you need to register Apollo Federation with your GraphQL service.


```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    // .AddApolloFederation() // use this instead if you want to opt-in to fed v1 
    .AddApolloFederationV2() 
    // register your types and services
    ;

var app = builder.Build();
app.MapGraphQL();
app.Run();
```

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
* `ComposeDirective` applicable on schema, see [`@composeDirective` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#composedirective)
* `Inaccessible` applicable on all type definitions, see [`@inaccessible` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#inaccessible)
* `InterfaceObject` applicable on objects, see [`@interfaceObject` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#interfaceobject)
* `KeyInterface` applicable on interfaces, see [entity interface `@key` documentation](https://www.apollographql.com/docs/federation/federated-types/interfaces)
* `Link` applicable on schema, see [`@link` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#the-link-directive)
* `Shareable` applicable on schema, see [`@shareable` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#shareable)

Entity resolution

* `Map` applicable on entity resolver method paramaters, allows you to map complex argument to a simpler representation value, e.g. `[Map("foo.bar")] string bar`
* `ReferenceResolver` applicable on static public methods to indicate entity resolver

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
* `ComposeDirective(name)` applicable on schema, see [`@composeDirective` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#composedirective)
* `Inaccessible` applicable on all type definitions, see [`@inaccessible` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#inaccessible)
* `InterfaceObject` applicable on objects, see [`@interfaceObject` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#interfaceobject)
* `Key(fieldset, resolvable?)` applicable on objects, see [`@key` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#key)
* `Link(url, [import]?)` applicable on schema, see [`@link` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#the-link-directive)
* `Shareable` applicable on fields and objects, see [`@shareable` documentation](https://www.apollographql.com/docs/federation/federated-types/federated-directives#shareable)

Entity resolution

* you have to provide `ResolveReferenceWith` function to be able to resolve the entities

### Advanced Use Cases

#### Generating schema at build time

See [HotChocolate documentation](https://chillicream.com/docs/hotchocolate/v13/server/command-line) for details on the server support for command line interface.

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGraphQLServer()
    .AddApolloFederationV2()
    // register your types and services
    ;

var app = builder.Build();
app.MapGraphQL();
app.RunWithGraphQLCommands();
```

You can then generate your schema by running

```shell
dotnet run -- schema export --output schema.graphql
```

#### `@composedDirective` usage

By default, Supergraph schema excludes all custom directives. The `@composeDirective`` is used to specify custom directives that should be preserved in the Supergraph schema.

`ApolloGraphQL.HotChocolate.Federation` provides common `FederatedSchema` class that automatically includes Apollo Federation v2 `@link` definition. When applying any custom
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

var app = builder.Build();
app.MapGraphQL();
app.Run();
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