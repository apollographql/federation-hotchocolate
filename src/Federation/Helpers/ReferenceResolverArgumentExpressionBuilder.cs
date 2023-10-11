using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using HotChocolate.Internal;
using HotChocolate.Language;
using HotChocolate.Resolvers;
using HotChocolate.Utilities;
using static ApolloGraphQL.HotChocolate.Federation.Constants.WellKnownContextData;

namespace ApolloGraphQL.HotChocolate.Federation.Helpers;

// TODO IParameterExpressionBuilder : IParameterHandler are part of HotChocolate.Internal package
internal class ReferenceResolverArgumentExpressionBuilder : IParameterExpressionBuilder
{
    private readonly MethodInfo _getValue =
        typeof(ArgumentParser).GetMethod(
            nameof(ArgumentParser.GetValue),
            BindingFlags.Static | BindingFlags.Public)!;

    private static readonly MethodInfo _getScopedState =
        typeof(ReferenceResolverArgumentExpressionBuilder).GetMethod(
            nameof(ReferenceResolverArgumentExpressionBuilder.GetScopedState))!;
    private static readonly MethodInfo _getScopedStateWithDefault =
        typeof(ReferenceResolverArgumentExpressionBuilder).GetMethod(
            nameof(ReferenceResolverArgumentExpressionBuilder.GetScopedStateWithDefault))!;

    public ArgumentKind Kind => ArgumentKind.LocalState;
    public bool CanHandle(ParameterInfo parameter) => true;

    public bool IsPure => false;

    public bool IsDefaultHandler => true;

    public Expression Build(ParameterExpressionBuilderContext context)
    {
        var param = context.Parameter;
        var path = Expression.Constant(GetPath(param), typeof(string[]));
        var dataKey = Expression.Constant(DataField, typeof(string));
        var typeKey = Expression.Constant(TypeField, typeof(string));
        var value = BuildGetter(param, dataKey, context.ResolverContext, typeof(IValueNode));
        var objectType = BuildGetter(param, typeKey, context.ResolverContext, typeof(ObjectType));
        var getValueMethod = _getValue.MakeGenericMethod(param.ParameterType);
        Expression getValue = Expression.Call(getValueMethod, value, objectType, path);
        return getValue;
    }

    private string[] GetPath(ParameterInfo parameter)
    {
        var path = parameter.GetCustomAttribute<MapAttribute>() is { } attr
          ? attr.Path.Split('.')
          : new[] { parameter.Name! };

        if (Required.Count == 0)
        {
            Required = new[] { path };
        }
        else if (Required.Count == 1)
        {
            var required = new List<string[]>(Required) { path };
            Required = required;
        }
        else if (Required is List<string[]> list)
        {
            list.Add(path);
        }

        return path;
    }
    public IReadOnlyList<string[]> Required { get; private set; } = Array.Empty<string[]>();

    private Expression BuildGetter(
        ParameterInfo parameter,
        ConstantExpression key,
        Expression context,
        Type? targetType = null)
    {
        targetType ??= parameter.ParameterType;

        var contextData = Expression.Property(context, ContextDataProperty);

        var getScopedState =
            parameter.HasDefaultValue
                ? _getScopedStateWithDefault.MakeGenericMethod(targetType)
                : _getScopedState.MakeGenericMethod(targetType);

        return parameter.HasDefaultValue
            ? Expression.Call(
                getScopedState,
                context,
                contextData,
                key,
                Expression.Constant(true, typeof(bool)),
                Expression.Constant(parameter.RawDefaultValue, targetType))
            : Expression.Call(
                getScopedState,
                context,
                contextData,
                key,
                Expression.Constant(false, typeof(bool)));
    }

    private PropertyInfo ContextDataProperty { get; } = typeof(IResolverContext).GetProperty(nameof(IResolverContext.LocalContextData))!;

    public static TContextData GetScopedState<TContextData>(
        IPureResolverContext context,
        IReadOnlyDictionary<string, object> contextData,
        string key,
        bool defaultIfNotExists = false)
        => GetScopedStateWithDefault<TContextData>(
            context, contextData, key, defaultIfNotExists, default);

    public static TContextData GetScopedStateWithDefault<TContextData>(
        IPureResolverContext context,
        IReadOnlyDictionary<string, object> contextData,
        string key,
        bool hasDefaultValue,
        TContextData defaultValue)
    {
        if (contextData.TryGetValue(key, out var value))
        {
            if (value is null)
            {
                return default;
            }

            if (value is TContextData v ||
                context.Service<ITypeConverter>().TryConvert(value, out v))
            {
                return v;
            }
        }
        else if (hasDefaultValue)
        {
            return defaultValue;
        }

        throw new ArgumentException(
            string.Format("The specified key `{0}` does not exist on `context.ScopedContextData", key));
    }
}
