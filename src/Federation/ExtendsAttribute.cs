using System.Reflection;
using HotChocolate.Types.Descriptors;

namespace ApolloGraphQL.HotChocolate.Federation;

/// <summary>
/// Extends directive is used to represent type extensions in the schema. Federated extended types should have 
/// corresponding @key directive defined that specifies primary key required to fetch the underlying object.
///
/// <example>
/// # extended from the Users service
/// type User @extends @key(fields: "email") {
///   email: String @external
///   reviews: [Review]
/// }
/// </example>
/// </summary>
public sealed class ExtendsAttribute : ObjectTypeDescriptorAttribute
{
    protected override void OnConfigure(
        IDescriptorContext context,
        IObjectTypeDescriptor descriptor,
        Type type)
        => descriptor.ExtendsType();
}
