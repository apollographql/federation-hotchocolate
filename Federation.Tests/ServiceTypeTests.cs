using System.Threading.Tasks;
using ApolloGraphQL.HotChocolate.Federation.Constants;
using HotChocolate;
using Snapshooter.Xunit;
using static ApolloGraphQL.HotChocolate.Federation.TestHelper;

namespace ApolloGraphQL.HotChocolate.Federation;

public class ServiceTypeTests : FederationTypesTestBase
{
    [Fact]
    public async Task TestServiceTypeEmptyQueryTypeSchemaFirst()
    {
        // arrange
        var schema = CreateSchemaBuilder()
            .AddApolloFederation()
            .AddDocumentFromString(
                @"type Query {

                }

                type Address @key(fields: ""matchCode"") {
                    matchCode: String!
                }")
            .Use(_ => _ => default)
            .Create();

        // act
        var entityType = schema.GetType<ServiceType>(WellKnownTypeNames.Service);

        // assert
        var value = await entityType.Fields[WellKnownFieldNames.Sdl].Resolver!(
            CreateResolverContext(schema));
        value.MatchSnapshot();
    }

    [Fact]
    public async Task TestServiceTypeTypeSchemaFirst()
    {
        // arrange
        var schema = CreateSchemaBuilder()
            .AddApolloFederation()
            .AddDocumentFromString(@"
                    type Query {
                        address: Address!
                    }

                    type Address @key(fields: ""matchCode"") {
                        matchCode: String!
                    }
                ")
            .Use(_ => _ => default)
            .Create();

        // act
        var entityType = schema.GetType<ServiceType>(WellKnownTypeNames.Service);

        // assert
        var value = await entityType.Fields[WellKnownFieldNames.Sdl].Resolver!(
            CreateResolverContext(schema));
        value.MatchSnapshot();
    }


    [Fact]
    public async Task TestServiceTypeEmptyQueryTypePureCodeFirst()
    {
        // arrange
        var schema = CreateSchemaBuilder()
            .ModifyOptions(opts => opts.QueryTypeName = "EmptyQuery")
            .AddApolloFederation()
            .AddType<Address>()
            .AddQueryType<EmptyQuery>()
            .Create();

        // act
        var entityType = schema.GetType<ServiceType>(WellKnownTypeNames.Service);

        // assert
        var value = await entityType.Fields[WellKnownFieldNames.Sdl].Resolver!(
            CreateResolverContext(schema));
        value.MatchSnapshot();
    }

    [Fact]
    public async Task TestServiceTypeTypePureCodeFirst()
    {
        // arrange
        var schema = CreateSchemaBuilder()
            .AddApolloFederation()
            .AddQueryType<Query>()
            .Create();

        // act
        var entityType = schema.GetType<ServiceType>(WellKnownTypeNames.Service);

        // assert
        var value = await entityType.Fields[WellKnownFieldNames.Sdl].Resolver!(
            CreateResolverContext(schema));
        value.MatchSnapshot();
    }

    public class EmptyQuery
    {
    }

    public class Query
    {
        public Address GetAddress(int id) => default!;
    }

    [Key("matchCode")]
    public class Address
    {
        public string? MatchCode { get; set; }
    }
}
