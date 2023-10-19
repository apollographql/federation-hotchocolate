using static ApolloGraphQL.HotChocolate.Federation.Constants.WellKnownArgumentNames;
using FieldSetTypeV1 = ApolloGraphQL.HotChocolate.Federation.One.FieldSetType;
using FieldSetTypeV2 = ApolloGraphQL.HotChocolate.Federation.Two.FieldSetType;

namespace ApolloGraphQL.HotChocolate.Federation;

internal static class DirectiveTypeDescriptorExtensions
{
    public static IDirectiveTypeDescriptor FieldsArgumentV1(
        this IDirectiveTypeDescriptor descriptor)
    {
        descriptor
            .Argument(Fields)
            .Type<NonNullType<FieldSetTypeV1>>();
        return descriptor;
    }

    public static IDirectiveTypeDescriptor FieldsArgumentV2(
        this IDirectiveTypeDescriptor descriptor)
    {
        descriptor
            .Argument(Fields)
            .Type<NonNullType<FieldSetTypeV2>>();
        return descriptor;
    }
}
