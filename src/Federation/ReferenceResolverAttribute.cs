using System.Reflection;
using ApolloGraphQL.HotChocolate.Federation.Descriptors;
using HotChocolate.Types.Descriptors;

namespace ApolloGraphQL.HotChocolate.Federation;

/// <summary>
/// The reference resolver enables your gateway's query planner to resolve a particular
/// entity by whatever unique identifier your other subgraphs use to reference it.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class ReferenceResolverAttribute : Attribute
{
    public void Configure(IDescriptorContext context, IObjectTypeDescriptor descriptor, MethodInfo method)
    {
        var entityResolverDescriptor = new EntityResolverDescriptor<object>(descriptor);
        entityResolverDescriptor.ResolveReferenceWith(method);
    }
}
