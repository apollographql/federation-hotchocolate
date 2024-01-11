using ApolloGraphQL.HotChocolate.Federation.Two;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSingleton<Data>();

builder.Services
    .AddGraphQLServer()
    .AddApolloFederationV2(new CustomSchema(FederationVersion.FEDERATION_25))
    .AddType<CustomDirectiveType>()
    .AddType<Inventory>()
    .AddQueryType<Query>()
    .RegisterService<Data>();

var app = builder.Build();

app.MapGraphQL("/");
app.RunWithGraphQLCommands(args);
