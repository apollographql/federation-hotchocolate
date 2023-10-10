namespace ApolloGraphQL.HotChocolate.Federation;

[AttributeUsage(
    AttributeTargets.Class
    | AttributeTargets.Struct
    | AttributeTargets.Method
    | AttributeTargets.Property
)]
public sealed class ShareableAttribute : Attribute
{
}
