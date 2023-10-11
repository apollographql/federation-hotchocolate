using HotChocolate.Types.Descriptors;

namespace ApolloGraphQL.HotChocolate.Federation.Two;

public abstract class SchemaTypeDescriptorAttribute : Attribute
{
    public abstract void OnConfigure(IDescriptorContext context, ISchemaTypeDescriptor descriptor, Type type);
}