using HotChocolate.Types.Descriptors;
using static ApolloGraphQL.HotChocolate.Federation.ThrowHelper;

namespace ApolloGraphQL.HotChocolate.Federation;

/// <summary>
/// The @key directive is used to indicate a combination of fields that
/// can be used to uniquely identify and fetch an object or interface.
/// <example>
/// type Product @key(fields: "upc") {
///   upc: UPC!
///   name: String
/// }
/// </example>
/// </summary>
public sealed class KeyAttribute : ObjectTypeDescriptorAttribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="KeyAttribute"/>.
    /// </summary>
    /// <param name="fieldSet">
    /// The field set that describes the key.
    /// Grammatically, a field set is a selection set minus the braces.
    /// </param>
    public KeyAttribute(string fieldSet) : this(fieldSet, null)
    {
    }

    public KeyAttribute(string fieldSet, bool? resolvable = null)
    {
        FieldSet = fieldSet;
        Resolvable = resolvable;
    }

    /// <summary>
    /// Gets the field set that describes the key.
    /// Grammatically, a field set is a selection set minus the braces.
    /// </summary>
    public string FieldSet { get; }

    public bool? Resolvable { get; }

    protected override void OnConfigure(IDescriptorContext context, IObjectTypeDescriptor descriptor, Type type)
    {
        if (string.IsNullOrEmpty(FieldSet))
        {
            throw Key_FieldSet_CannotBeEmpty(type);
        }
        descriptor.Key(FieldSet, Resolvable);
    }
}
