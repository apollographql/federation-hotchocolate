# HotChocolate Federation Compatibility using Code-First approach

Example HotChocolate subgraph (configured using explicit binding and code first approach) used for testing [Apollo Federation Subgraph Compatibility](https://github.com/apollographql/apollo-federation-subgraph-compatibility).

## Usage

Install compatibility script

```shell
npm install --dev @apollo/federation-subgraph-compatibility
```

Generate test schema

```shell
dotnet run --project Compatibility/CodeFirst/CodeFirst.csproj schema export --output products.graphql
```

Run compatibility tests

```shell
npx fedtest docker --compose Compatibility/CodeFirst/docker-compose.yaml --schema Compatibility/CodeFirst/products.graphql
```

See [compatibility script documentation](https://www.npmjs.com/package/@apollo/federation-subgraph-compatibility) for additional details.