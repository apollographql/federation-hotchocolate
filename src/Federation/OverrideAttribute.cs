using System.Reflection;
using HotChocolate.Types.Descriptors;

namespace ApolloGraphQL.HotChocolate.Federation;

public sealed class OverrideAttribute : ObjectFieldDescriptorAttribute
{

    public OverrideAttribute(string from)
    {
        From = from;
    }

    public string From { get; }

    protected override void OnConfigure(
        IDescriptorContext context,
        IObjectFieldDescriptor descriptor,
        MemberInfo member)
    {
        descriptor.Override(From);
    }
}
