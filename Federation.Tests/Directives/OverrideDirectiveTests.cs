using System.Linq;
using ApolloGraphQL.HotChocolate.Federation.Constants;
using ApolloGraphQL.HotChocolate.Federation.Two;
using HotChocolate;
using HotChocolate.Types;
using HotChocolate.Utilities;
using Snapshooter.Xunit;

namespace ApolloGraphQL.HotChocolate.Federation.Directives;

public class OverrideDirectiveTests : FederationTypesTestBase
{
    [Fact]
    public void AddOverrideDirective_EnsureAvailableInSchema()
    {
        var schema = CreateSchema(b => b.AddDirectiveType<OverrideDirectiveTypeV27>());

        var directive =
            schema.DirectiveTypes.FirstOrDefault(
                t => t.Name.EqualsOrdinal(WellKnownTypeNames.Override));

        Assert.NotNull(directive);
        Assert.IsType<OverrideDirectiveTypeV27>(directive);
        Assert.Equal(WellKnownTypeNames.Override, directive!.Name);
        Assert.Equal(2, directive.Arguments.Count);
        // TODO: possible to assert NonNullType<StringType>?
        AssertDirectiveHasArgumentOfType<NonNullType>(directive, WellKnownArgumentNames.From);
        AssertDirectiveHasArgumentOfType<StringType>(directive, WellKnownArgumentNames.Label);
        Assert.Equal(DirectiveLocation.FieldDefinition, directive.Locations);
    }

    [Fact]
    public void AnnotateOverrideToFieldSchemaFirst()
    {
        // arrange
        Snapshot.FullName();

        var schema = CreateSchemaBuilder()
            .AddDocumentFromString(
                @"
                    type Review @key(fields: ""id"") {
                        id: Int!
                    }

                    type Query {
                        someField(a: Int): Review @override(from: ""reviews"", label: ""percent(50)"")
                    }
                ")
            .AddDirectiveType<KeyDirectiveType>()
            .AddDirectiveType<OverrideDirectiveTypeV27>()
            .AddType<FieldSetType>()
            .Use(_ => _ => default)
            .Create();

        // act
        var testType = schema.GetType<ObjectType>("Query");

        // assert
        Assert.Collection(testType.Fields.Single(field => field.Name == "someField").Directives,
            overrideDirective =>
            {
                Assert.Equal(WellKnownTypeNames.Override, overrideDirective.Type.Name);
                Assert.Equal("from", overrideDirective.AsSyntaxNode().Arguments[0].Name.ToString());
                Assert.Equal("\"reviews\"", overrideDirective.AsSyntaxNode().Arguments[0].Value.ToString());
                Assert.Equal("label", overrideDirective.AsSyntaxNode().Arguments[1].Name.ToString());
                Assert.Equal("\"percent(50)\"", overrideDirective.AsSyntaxNode().Arguments[1].Value.ToString());
            });

        schema.ToString().MatchSnapshot();
    }

    [Fact]
    public void AnnotateOverrideToFieldCodeFirst()
    {
        // arrange
        Snapshot.FullName();

        var schema = CreateSchemaBuilder()
            .AddType(
                new ObjectType(o =>
                {
                    o.Name("Review").Key("id");
                    o.Field("id").Type<IntType>();
                }))
            .AddQueryType(
                new ObjectType(o =>
                {
                    o.Name("Query");
                    o.Field("someField")
                        .Argument("a", a => a.Type<IntType>())
                        .Type("Review")
                        .Override("reviews", "percent(50)");
                }))
            .AddType<FieldSetType>()
            .AddDirectiveType<KeyDirectiveType>()
            .AddDirectiveType<OverrideDirectiveTypeV27>()
            .Use(_ => _ => default)
            .Create();

        // act
        var testType = schema.GetType<ObjectType>("Query");

        // assert
        Assert.Collection(
            testType.Fields.Single(field => field.Name == "someField").Directives,
            overrideDirective =>
            {
                Assert.Equal(
                    WellKnownTypeNames.Override,
                    overrideDirective.Type.Name);
                Assert.Equal(
                    "from",
                    overrideDirective.AsSyntaxNode().Arguments[0].Name.ToString());
                Assert.Equal(
                    "\"reviews\"",
                    overrideDirective.AsSyntaxNode().Arguments[0].Value.ToString());
                Assert.Equal(
                    "label",
                    overrideDirective.AsSyntaxNode().Arguments[1].Name.ToString());
                Assert.Equal(
                    "\"percent(50)\"",
                    overrideDirective.AsSyntaxNode().Arguments[1].Value.ToString());
            });

        schema.ToString().MatchSnapshot();
    }

    [Fact]
    public void AnnotateOverrideToClassAttributePureCodeFirst()
    {
        // arrange
        Snapshot.FullName();

        var schema = CreateSchemaBuilder()
            .AddApolloFederationV2()
            .AddQueryType<Query>()
            .Create();

        // act
        var queryType = schema.GetType<ObjectType>("Query");

        // assert
        Assert.Collection(
            queryType.Fields.Single(field => field.Name == "someField").Directives,
            overrideDirective =>
            {
                Assert.Equal(
                    WellKnownTypeNames.Override,
                    overrideDirective.Type.Name);
                Assert.Equal(
                    "from",
                    overrideDirective.AsSyntaxNode().Arguments[0].Name.ToString());
                Assert.Equal(
                    "\"reviews\"",
                    overrideDirective.AsSyntaxNode().Arguments[0].Value.ToString());
                Assert.Equal(
                    "label",
                    overrideDirective.AsSyntaxNode().Arguments[1].Name.ToString());
                Assert.Equal(
                    "\"percent(50)\"",
                    overrideDirective.AsSyntaxNode().Arguments[1].Value.ToString());
            });

        schema.ToString().MatchSnapshot();
    }

    public class Query
    {
        [ProgressiveOverride("reviews", "percent(50)")]
        public Review SomeField(int id) => default!;
    }

    [Key("id")]
    public class Review
    {
        public int Id { get; set; }
    }
}
