using System.Threading.Tasks;
using ApolloGraphQL.HotChocolate.Federation.CertificationSchema.AnnotationBased.Types;
using HotChocolate.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace ApolloGraphQL.HotChocolate.Federation.CertificationSchema.AnnotationBased;

public static class SchemaSetup
{
    public static async Task<IRequestExecutor> CreateAsync()
        => await new ServiceCollection()
            .AddSingleton<Data>()
            .AddGraphQL()
            .AddApolloFederation()
            .AddQueryType<Query>()
            .RegisterService<Data>()
            .BuildRequestExecutorAsync();
}
