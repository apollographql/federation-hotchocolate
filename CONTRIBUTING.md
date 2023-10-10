# Contributing to Federation HotChocolate

The Apollo team welcomes contributions of all kinds, including bug reports, documentation, test cases, bug fixes, and features. There are just a few guidelines you need to follow which are described in detail below.

If you want to discuss the project or just say hi, stop by [the Apollo community forums](https://community.apollographql.com/) or our [Discord server](https://discord.gg/graphos)

## Workflow

We love Github issues! Before working on any new features, please open an issue so that we can agree on the direction, and hopefully avoid investing a lot of time on a feature that might need reworking.

Small pull requests for things like typos, bugfixes, etc are always welcome.

Please note that we will not accept pull requests for style changes.

### Fork this repo

You should create a fork of this project in your account and work from there. You can create a fork by clicking the fork button in GitHub.

### One feature, one branch

Work for each new feature/issue should occur in its own branch. To create a new branch from the command line:

```shell
git checkout -b my-new-feature
```
where "my-new-feature" describes what you're working on.

### Verify your changes locally

You can use `dotnet` to build all the modules from the root directory

```shell
dotnet restore
dotnet build
```

### Add tests for any bug fixes or new functionality

#### Unit Tests

We are using `ApolloGraphQL.Federation.HotChocolate.Tests` project for our unit tests. This ensures we have good code coverage and can easily test all cases of schema federation.

To run tests:

```shell
dotnet test
```

### Add documentation for new or updated functionality

Please add appropriate javadocs in the source code and ask the maintainers to update the documentation with any relevant information.

### Merging your contribution

Create a new pull request (with appropriate labels) and your code will be reviewed by the maintainers. They will confirm at least the following:

- Tests run successfully (unit, coverage, integration, style)
- Contribution policy has been followed
- Apollo [CLA](https://contribute.apollographql.com/) is signed

A maintainer will need to sign off on your pull request before it can be merged.

## Releasing

TODO
