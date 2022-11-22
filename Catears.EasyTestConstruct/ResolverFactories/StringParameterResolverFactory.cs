﻿using System.Reflection;
using Catears.EasyTestConstruct.Providers;
using Catears.EasyTestConstruct.Resolvers;

namespace Catears.EasyTestConstruct.ResolverFactories;

internal class StringParameterResolverFactory : IParameterResolverFactory
{
    public void CreateParameterResolver(ParameterInfo parameterInfo, Action<IParameterResolver> finished, Action next)
    {
        var type = parameterInfo.ParameterType;
        if (type == typeof(string))
        {
            var options = StringProviderOptions.Default with
            {
                VariableName = parameterInfo.Name,
                VariableType = type.Name
            };
            finished(new StringResolver(options));
        }
        else
        {
            next();
        }
    }

    public bool TryCreateParameterResolver(ParameterInfo parameterInfo, out IParameterResolver? resolver)
    {
        var type = parameterInfo.ParameterType;
        if (type == typeof(string))
        {
            var options = StringProviderOptions.Default with
            {
                VariableName = parameterInfo.Name,
                VariableType = type.Name
            };
            resolver = new StringResolver(options);
            return true;
        }

        resolver = null;
        return false;
    }
}