using HotChocolate.Types.Descriptors;

namespace ApolloGraphQL.HotChocolate.Federation;

public sealed class InterfaceObjectAttribute : ObjectTypeDescriptorAttribute
{
    protected override void OnConfigure(IDescriptorContext context, IObjectTypeDescriptor descriptor, Type type)
    {
        descriptor.InterfaceObject();
    }
}
