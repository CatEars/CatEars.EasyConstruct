﻿using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Catears.EasyConstruct.Registrators;

internal static class AdvancedConstructorRegistrationMethod
{
    public static void Register(IServiceCollection serviceCollection, Type serviceType, ConstructorInfo constructor)
    {
        var parameterDescriptors = constructor.GetParameters();
        var sortedByPosition = parameterDescriptors.OrderBy(paramInfo => paramInfo.Position);
        var parameterResolvers = sortedByPosition
            .Select(ParameterResolverFactory.GetResolverForType)
            .ToList();

        serviceCollection.AddTransient(serviceType, services =>
        {
            var parameters = parameterResolvers.Select(resolver => resolver.ResolveParameter(services));
            return constructor.Invoke(parameters.ToArray());
        });
    }
}