var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<Data>();

builder.Services
    .AddGraphQLServer()
    // TODO there is no way to programmatically configure custom schema as we don't have option to access schema object
    // ISchemaDirective can be used to specify directive definition but I don't see an option to provide applied value
    .AddApolloFederationV2(new CustomSchema())
    .AddType<CustomDirectiveType>()
    .AddType<InventoryType>()
    .AddQueryType<QueryType>()
    .ModifyOptions(options => options.DefaultBindingBehavior = BindingBehavior.Explicit)
    .RegisterService<Data>();

var app = builder.Build();

app.MapGraphQL("/");
app.RunWithGraphQLCommands(args);
