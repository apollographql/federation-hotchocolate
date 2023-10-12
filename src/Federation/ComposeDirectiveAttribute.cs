using ApolloGraphQL.HotChocolate.Federation.Constants;
using ApolloGraphQL.HotChocolate.Federation.Two;
using HotChocolate.Types.Descriptors;
using HotChocolate.Language;
using static ApolloGraphQL.HotChocolate.Federation.ThrowHelper;

namespace ApolloGraphQL.HotChocolate.Federation;

public sealed class ComposeDirectiveAttribute : SchemaTypeDescriptorAttribute
{
    public ComposeDirectiveAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public override void OnConfigure(IDescriptorContext context, ISchemaTypeDescriptor descriptor, Type type)
    {
        if (string.IsNullOrEmpty(Name))
        {
            throw ComposeDirective_Name_CannotBeEmpty(type);
        }
        descriptor.Directive(
            WellKnownTypeNames.ComposeDirective,
            new ArgumentNode(
                WellKnownArgumentNames.Name,
                new StringValueNode(Name)
                )
            );
    }
}
