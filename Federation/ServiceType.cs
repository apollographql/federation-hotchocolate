using ApolloGraphQL.HotChocolate.Federation.Constants;
using ApolloGraphQL.HotChocolate.Federation.Properties;
using HotChocolate.Resolvers;
using FederationSchemaPrinter = ApolloGraphQL.HotChocolate.Federation.One.FederationSchemaPrinter;

namespace ApolloGraphQL.HotChocolate.Federation;

/// <summary>
/// A new object type called _Service must be created.
/// This type must have an sdl: String! field which exposes the SDL of the service's schema.
///
/// This SDL (schema definition language) is a printed version of the service's
/// schema including the annotations of federation directives. This SDL does not
/// include the additions of the federation spec.
/// </summary>
public class ServiceType : ObjectType
{
    public ServiceType(bool isV2 = false)
    {
        IsV2 = isV2;
    }

    public bool IsV2 { get; }

    protected override void Configure(IObjectTypeDescriptor descriptor)
        => descriptor
            .Name(WellKnownTypeNames.Service)
            .Description(FederationResources.ServiceType_Description)
            .Field(WellKnownFieldNames.Sdl)
            .Type<NonNullType<StringType>>()
            .Resolve(resolver => PrintSchemaSDL(resolver, IsV2));

    private string PrintSchemaSDL(IResolverContext resolver, bool isV2)
    {
        if (isV2)
        {
            return SchemaPrinter.Print(resolver.Schema);
        }
        else
        {
            return FederationSchemaPrinter.Print(resolver.Schema);
        }
    }
}
