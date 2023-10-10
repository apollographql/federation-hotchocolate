using static ApolloGraphQL.Federation.HotChocolate.Constants.WellKnownArgumentNames;

namespace ApolloGraphQL.Federation.HotChocolate;

internal static class DirectiveTypeDescriptorExtensions
{
    public static IDirectiveTypeDescriptor FieldsArgument(
        this IDirectiveTypeDescriptor descriptor)
    {
        descriptor
            .Argument(Fields)
            .Type<NonNullType<FieldSetType>>();

        return descriptor;
    }
}
