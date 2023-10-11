using HotChocolate.Configuration;
using HotChocolate.Types.Descriptors;
using HotChocolate.Types.Descriptors.Definitions;

namespace ApolloGraphQL.HotChocolate.Federation.Two;

[Link("https://specs.apollo.dev/federation/v2.5", new string[] {
                "@composeDirective",
                "@extends",
                "@external",
                "@key",
                "@inaccessible",
                "@interfaceObject",
                "@override",
                "@provides",
                "@requires",
                "@shareable",
                "@tag",
                "FieldSet"
})]
public class FederatedSchema : Schema
{
    private IDescriptorContext _context = default!;
    protected override void OnAfterInitialize(ITypeDiscoveryContext context, DefinitionBase definition)
    {
        base.OnAfterInitialize(context, definition);
        _context = context.DescriptorContext;
    }

    protected override void Configure(ISchemaTypeDescriptor descriptor)
    {
        var schemaType = this.GetType();
        if (schemaType.IsDefined(typeof(SchemaTypeDescriptorAttribute), true))
        {
            foreach (var attribute in schemaType.GetCustomAttributes(true))
            {
                if (attribute is SchemaTypeDescriptorAttribute casted)
                {
                    casted.OnConfigure(_context, descriptor, schemaType);
                }
            }
        }
    }
}