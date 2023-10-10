# Hot Chocolate Federation Examples

[Apollo Federation](https://www.apollographql.com/docs/federation/) example using HotChocolate and [Apollo Router](https://www.apollographql.com/docs/router/).

The repository contains `AnnotationBased` and `CodeFirst` examples of of following subgraphs:

1. `Accounts`: HotChocolate service providing the federated `User` type
2. `Inventory`: HotChocolate service that extends the `Product` type with additional inventory information
3. `Products`: HotChocolate service providing the federated `Product` type
4. `Reviews`: HotChocolate service that extends the `Product` and `User` types with `Reviews`

## Running example locally

1. Install dependencies by running `dotnet restore` from root directory
2. Build projects locally `dotnet build` from root directory
3. Start individual subgraph by running (in separate shells, example below is for `AnnotationBased` projects)
   ```shell
   dotnet run --project examples/AnnotationBased/Accounts/Accounts.csproj
   dotnet run --project examples/AnnotationBased/Inventory/Inventory.csproj
   dotnet run --project examples/AnnotationBased/Products/Products.csproj
   dotnet run --project examples/AnnotationBased/Reviews/Reviews.csproj
   ```
4. Start Federated Router
    1. Install [rover CLI](https://www.apollographql.com/docs/rover/getting-started)
    2. Start router and compose products schema using [rover dev command](https://www.apollographql.com/docs/rover/commands/dev)

    ```shell
    rover dev --supergraph-config examples/supergraph.yaml
    ```

5. Open http://localhost:4000 for the query editor

Example federated query

```graphql
query ExampleQuery {
  me {
    id
  }
  topProducts {
    inStock
    name
    price
    reviews {
      author {
        id
        name
      }
      body
      id
    }
  }
}
```
