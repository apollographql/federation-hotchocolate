using static ApolloGraphQL.HotChocolate.Federation.Constants.WellKnownArgumentNames;

namespace ApolloGraphQL.HotChocolate.Federation;

internal static class DirectiveTypeDescriptorExtensions
{
    public static IDirectiveTypeDescriptor FieldsArgumentV1(
        this IDirectiveTypeDescriptor descriptor)
    {
        descriptor
            .Argument(Fields)
            .Type<NonNullType<FieldSetType>>();

        return descriptor;
    }
}
