using System;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.Language;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using static ApolloGraphQL.HotChocolate.Federation.Constants.WellKnownContextData;
using static ApolloGraphQL.HotChocolate.Federation.TestHelper;

namespace ApolloGraphQL.HotChocolate.Federation;

public class ReferenceResolverAttributeTests : FederationTypesTestBase
{
    [Fact]
    public async void SimpleKey()
    {
        var schema = CreateSchemaBuilder()
            .AddApolloFederation()
            .AddQueryType<QueryWithSingleKey>()
            .Create();

        var type = schema.GetType<ObjectType>(nameof(KeyEntity));

        var resultId = await ResolveRef(schema, type, new(new ObjectFieldNode("id", "id_123")));

        Assert.Equal("id_123", Assert.IsType<KeyEntity>(resultId).Id);
    }

    [Fact]
    public async void MultiKey()
    {
        // arrange
        var schema = CreateSchemaBuilder()
            .AddApolloFederation()
            .AddQueryType<QueryWithMultiKey>()
            .Create();

        var type = schema.GetType<ObjectType>(nameof(MultiKeyEntity));

        // act
        var resultId = await ResolveRef(schema, type, new(new ObjectFieldNode("id", "id_123")));
        var resultSku = await ResolveRef(schema, type, new(new ObjectFieldNode("sku", "sku_123")));

        // assert
        Assert.Equal("id_123", Assert.IsType<MultiKeyEntity>(resultId).Id);
        Assert.Equal("sku_123", Assert.IsType<MultiKeyEntity>(resultSku).Sku);
    }

    [Fact]
    public async void CompositeKey()
    {
        var schema = CreateSchemaBuilder()
            .AddApolloFederation()
            .AddQueryType<QueryWithCompositeKey>()
            .Create();

        var type = schema.GetType<ObjectType>(nameof(CompositeKeyEntity));

        // act
        var result = await ResolveRef(schema, type, new(new ObjectFieldNode("id", "id_123"), new ObjectFieldNode("sku", "sku_123")));

        // assert
        Assert.Equal("id_123", Assert.IsType<CompositeKeyEntity>(result).Id);
        Assert.Equal("sku_123", Assert.IsType<CompositeKeyEntity>(result).Sku);
    }

    [Fact]
    public async void CompositeObjectKey()
    {
        var schema = CreateSchemaBuilder()
            .AddApolloFederation()
            .AddQueryType<QueryWithObjectKey>()
            .Create();

        var type = schema.GetType<ObjectType>(nameof(ObjectKeyEntity));
        var result = await ResolveRef(schema, type, new ObjectValueNode(new ObjectFieldNode("foo", new ObjectValueNode(new ObjectFieldNode("id", "id_123")))));

        Assert.Equal("id_123", Assert.IsType<ObjectKeyEntity>(result).Foo.Id);
    }

    private async ValueTask<object?> ResolveRef(
        ISchema schema,
        ObjectType type,
        ObjectValueNode representation)
    {
        var inClassResolverContextObject = type.ContextData[EntityResolver];
        Assert.NotNull(inClassResolverContextObject);
        var inClassResolverDelegate =
            Assert.IsType<FieldResolverDelegate>(inClassResolverContextObject);
        var context = CreateResolverContext(schema, type);

        context.SetLocalState(DataField, representation);
        context.SetLocalState(TypeField, type);

        var entity = await inClassResolverDelegate.Invoke(context);

        if (entity is not null &&
            type!.ContextData.TryGetValue(ExternalSetter, out var value) &&
            value is Action<ObjectType, IValueNode, object> setExternals)
        {
            setExternals(type, representation!, entity);
        }

        return entity;
    }

    // TEST SCHEMAS BELOW

    public class QueryWithSingleKey
    {
        public KeyEntity SingleKey { get; set; } = default!;
    }

    [Key("id")]
    public class KeyEntity
    {
        public string Id { get; set; } = default!;

        [ReferenceResolver]
        public static Task<KeyEntity> GetByIdAsync(string id)
            => Task.FromResult(new KeyEntity { Id = id });
    }

    public class QueryWithMultiKey
    {
        public MultiKeyEntity MultiKey { get; set; } = default!;
    }


    [Key("id")]
    [Key("sku")]
    public class MultiKeyEntity
    {
        public string Id { get; set; } = default!;

        public string Sku { get; set; } = default!;

        [ReferenceResolver]
        public static Task<MultiKeyEntity> GetByIdAsync(string id)
            => Task.FromResult(new MultiKeyEntity { Id = id });

        [ReferenceResolver]
        public static Task<MultiKeyEntity> GetBySkuAsync(string sku)
            => Task.FromResult(new MultiKeyEntity { Sku = sku });
    }

    public class QueryWithCompositeKey
    {
        public CompositeKeyEntity CompositeKey { get; set; } = default!;
    }

    [Key("id sku")]
    public class CompositeKeyEntity
    {
        public string Id { get; set; } = default!;

        public string Sku { get; set; } = default!;

        [ReferenceResolver]
        public static Task<CompositeKeyEntity> GetByIdAndSkuAsync(string id, string sku)
            => Task.FromResult(new CompositeKeyEntity { Id = id, Sku = sku });
    }

    public class QueryWithObjectKey
    {
        public ObjectKeyEntity ObjectKey { get; set; } = default!;
    }

    [Key("id sku")]
    public class ObjectKeyEntity
    {
        public ObjectKey Foo { get; set; } = default!;

        [ReferenceResolver]
        public static Task<ObjectKeyEntity> GetByFooIdAsync([Map("foo.id")] string id)
            => Task.FromResult(new ObjectKeyEntity { Foo = new ObjectKey { Id = id } });
    }

    public class ObjectKey
    {
        public string Id { get; set; } = default!;
    }
}
