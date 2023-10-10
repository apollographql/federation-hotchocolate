using System.Threading.Tasks;
using ApolloGraphQL.Federation.HotChocolate.Constants;
using HotChocolate;
using Snapshooter.Xunit;
using Xunit;
using static ApolloGraphQL.Federation.HotChocolate.TestHelper;

namespace ApolloGraphQL.Federation.HotChocolate;

public class ServiceTypeTests
{
    [Fact]
    public async Task TestServiceTypeEmptyQueryTypeSchemaFirst()
    {
        // arrange
        var schema = SchemaBuilder.New()
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
        var schema = SchemaBuilder.New()
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
        var schema = SchemaBuilder.New()
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
        var schema = SchemaBuilder.New()
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
