var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<Data>();

builder.Services
    .AddGraphQLServer()
    .AddApolloFederationV2(schemaConfiguration: s =>
    {
        s.Link("https://myspecs.dev/myCustomDirective/v1.0", ["@custom"]);
        s.ComposeDirective("@custom");
    })
    .AddType<CustomDirectiveType>()
    .AddType<InventoryType>()
    .AddQueryType<QueryType>()
    .ModifyOptions(options => options.DefaultBindingBehavior = BindingBehavior.Explicit)
    .RegisterService<Data>();

var app = builder.Build();

app.MapGraphQL("/");
app.RunWithGraphQLCommands(args);
