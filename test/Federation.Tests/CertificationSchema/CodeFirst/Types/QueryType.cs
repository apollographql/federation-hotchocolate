﻿using System.Linq;
using HotChocolate.Types;

namespace ApolloGraphQL.HotChocolate.Federation.CertificationSchema.CodeFirst.Types;

public class QueryType : ObjectType
{
    protected override void Configure(IObjectTypeDescriptor descriptor)
    {
        descriptor
            .ExtendServiceType()
            .Name("Query")
            .Field("product")
            .Argument("id", a => a.Type<NonNullType<IdType>>())
            .Type<ProductType>()
            .Resolve(ctx =>
            {
                var id = ctx.ArgumentValue<string>("id");
                return ctx.Service<Data>().Products.FirstOrDefault(t => t.Id.Equals(id));
            });
    }
}
