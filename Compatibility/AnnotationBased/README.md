# HotChocolate Federation Compatibility usign AnnotationBased approach

Example HotChocolate subgraph (configured using Attributes) used for testing [Apollo Federation Subgraph Compatibility](https://github.com/apollographql/apollo-federation-subgraph-compatibility).

## Usage

Install compatibility script

```shell
npm install --dev @apollo/federation-subgraph-compatibility
```

Generate test schema

```shell
dotnet run --project Compatibility/AnnotationBased/AnnotationBased.csproj schema export --output products.graphql
```

Run compatibility tests

```shell
npx fedtest docker --compose Compatibility/AnnotationBased/docker-compose.yaml --schema Compatibility/AnnotationBased/products.graphql
```

See [compatibility script documentation](https://www.npmjs.com/package/@apollo/federation-subgraph-compatibility) for additional details.