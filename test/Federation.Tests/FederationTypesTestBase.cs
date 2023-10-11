using System;
using ApolloGraphQL.HotChocolate.Federation.One;
using HotChocolate;
using HotChocolate.Types;
using Xunit;

namespace ApolloGraphQL.HotChocolate.Federation;

public class FederationTypesTestBase
{
    protected ISchema CreateSchema(Action<ISchemaBuilder> configure)
    {
        var builder =
            SchemaBuilder.New()
                .AddQueryType(
                    c =>
                    {
                        c.Name("Query");
                        c.Field("foo").Type<StringType>().Resolve("bar");
                    });

        configure(builder);

        return builder.Create();
    }

    protected void AssertDirectiveHasFieldsArgument(DirectiveType directive)
    {
        Assert.Collection(
            directive.Arguments,
            t =>
            {
                Assert.Equal("fields", t.Name);
                Assert.IsType<FieldSetType>(Assert.IsType<NonNullType>(t.Type).Type);
            }
        );
    }
}
