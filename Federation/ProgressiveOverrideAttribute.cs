using System.Reflection;
using HotChocolate.Types.Descriptors;

namespace ApolloGraphQL.HotChocolate.Federation;

/// <summary>
/// <code>
/// directive @override(from: String!, label: String) on FIELD_DEFINITION
/// </code>
///
/// The @override directive is used to indicate that the current subgraph is
/// taking responsibility for resolving the marked field away from the subgraph
/// specified in the from argument. Name of the subgraph to be overridden has to
/// match the name of the subgraph that was used to publish their schema. As of
/// federation v2.7, Enterprise router users can use the optional `label`
/// argument to progressively roll out a field migration.
///
/// NOTE: Only available in Federation v2
/// <example>
/// type Foo @key(fields: "id") {
///   id: ID!
///   description: String @override(from: "BarSubgraph")
/// }
/// </example>
/// </summary>
public sealed class ProgressiveOverrideAttribute : ObjectFieldDescriptorAttribute
{

    /// <summary>
    /// Initializes new instance of <see cref="ProgressiveOverrideAttribute"/> 
    /// </summary>
    /// <param name="from">
    /// Name of the subgraph to be overridden
    /// </param>
    /// <param name="label">
    /// Label used to progressively roll out a field migration
    /// </param>
    public ProgressiveOverrideAttribute(string from, string label)
    {
        From = from;
        Label = label;
    }

    /// <summary>
    /// Get name of the subgraph to be overridden.
    /// </summary>
    public string From { get; }
    
    /// <summary>
    /// Get the override label.
    /// </summary>
    public string Label { get; }

    protected override void OnConfigure(
        IDescriptorContext context,
        IObjectFieldDescriptor descriptor,
        MemberInfo member)
    {
        descriptor.Override(From, Label);
    }
}
