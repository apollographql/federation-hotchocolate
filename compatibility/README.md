# HotChocolate Federation Compatibility

Example HotChocolate subgraph used for testing [Apollo Federation Subgraph Compatibility](https://github.com/apollographql/apollo-federation-subgraph-compatibility).

## Usage

Install compatibility script

```shell
npm install --dev @apollo/federation-subgraph-compatibility
```

Generate test schema

```shell
dotnet run --project compatibility/Products.csproj schema export --output products.graphql
```

Run compatibility tests

```shell
npx fedtest docker --compose compatibility/docker-compose.yaml --schema compatibility/products.graphql
```

See [compatibility script documentation](https://www.npmjs.com/package/@apollo/federation-subgraph-compatibility) for additional details.