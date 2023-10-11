using System.Collections.Generic;
using ApolloGraphQL.HotChocolate.Federation.Constants;
using ApolloGraphQL.HotChocolate.Federation.Two;
using HotChocolate.Types.Descriptors;
using HotChocolate.Language;
using static ApolloGraphQL.HotChocolate.Federation.ThrowHelper;
using System.Linq;

namespace ApolloGraphQL.HotChocolate.Federation;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = true, AllowMultiple = true)]
public sealed class LinkAttribute : SchemaTypeDescriptorAttribute
{
    public LinkAttribute(string url)
    {
        Url = url;
        Import = null;
    }

    public LinkAttribute(string url, string?[]? import)
    {
        Url = url;
        Import = import;
    }

    public string Url { get; }

    public string?[]? Import { get; }

    public override void OnConfigure(IDescriptorContext context, ISchemaTypeDescriptor descriptor, Type type)
    {
        if (string.IsNullOrEmpty(Url))
        {
            throw Link_Url_CannotBeEmpty(type);
        }
        descriptor.Directive(new Link(Url, Import?.ToList()));
    }
}
