var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<Data>();

builder.Services
    .AddGraphQLServer()
    .AddApolloFederationV2()
    .AddType<CustomDirectiveType>()
    .AddType<InventoryType>()
    .AddQueryType<QueryType>()
    .ApplyComposeDirective("https://myspecs.dev/myCustomDirective/v1.0", "@custom")
    .ModifyOptions(options => options.DefaultBindingBehavior = BindingBehavior.Explicit)
    .RegisterService<Data>();

var app = builder.Build();

app.MapGraphQL("/");
app.RunWithGraphQLCommands(args);
