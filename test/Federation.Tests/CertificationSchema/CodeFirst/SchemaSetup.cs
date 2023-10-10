using System.Threading.Tasks;
using ApolloGraphQL.Federation.HotChocolate.CertificationSchema.CodeFirst.Types;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace ApolloGraphQL.Federation.HotChocolate.CertificationSchema.CodeFirst;

public static class SchemaSetup
{
    public static async Task<IRequestExecutor> CreateAsync()
        => await new ServiceCollection()
            .AddSingleton<Data>()
            .AddGraphQL()
            .AddApolloFederation()
            .AddQueryType<QueryType>()
            .RegisterService<Data>()
            .BuildRequestExecutorAsync();
}
