using System;
using ApolloGraphQL.HotChocolate.Federation.One;
using HotChocolate;
using HotChocolate.Types;

namespace ApolloGraphQL.HotChocolate.Federation;

public class FederationTypesTestBase
{
    protected ISchemaBuilder CreateSchemaBuilder()
    {
        return SchemaBuilder.New()
            .ModifyOptions(opt =>
                {
                    // tag is added by default
                    opt.EnableTag = false;
                    // this is set after we process our federated schema extensions
                    if (opt.QueryTypeName is null)
                    {
                        opt.QueryTypeName = "Query";
                    }
                });
    }

    protected ISchema CreateSchema(Action<ISchemaBuilder> configure)
    {
        var builder =
            CreateSchemaBuilder()
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
