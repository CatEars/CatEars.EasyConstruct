﻿using System.Reflection;
using Catears.EasyTestConstruct.Providers;
using Catears.EasyTestConstruct.ResolverFactories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Catears.EasyTestConstruct;

public class BuildContext
{
    private ServiceCollection ServiceCollection { get; } =  new();

    public BuildContext()
    {
        ServiceCollection.AddScoped<IntProvider>();
    }
    
    public void Register<T>() where T : class
    {
        var typeDescriptor = typeof(T);
        var constructors = typeDescriptor.GetConstructors();
        if (constructors.Length == 0)
        {
            ServiceCollection.AddScoped<T>();
        }
        else if (constructors.Length == 1)
        {
            RegisterAdvancedBuilder<T>(constructors[0]);
        }
        else
        {
            throw new NotImplementedException("Objects with multiple constructors are not supported");
        }
    }

    public void Register<T>(Func<IServiceProvider, T> builder) where T : class
    {
        ServiceCollection.AddScoped(builder);
    }

    public void Register<T>(Func<T> builder) where T : class
    {
        Register(_ => builder());
    }

    private void RegisterAdvancedBuilder<T>(ConstructorInfo constructor) where T : class
    {
        var parameterDescriptors = constructor.GetParameters();
        var sortedByPosition = parameterDescriptors.OrderBy(paramInfo => paramInfo.Position);
        var parameterResolvers = sortedByPosition
            .Select(DefaultParameterResolverFactoryChain.FirstLink.CreateParameterResolverOrThrow)
            .ToList();

        ServiceCollection.AddScoped(services =>
        {
            var parameters = parameterResolvers.Select(resolver => resolver.ResolveParameter(services));
            return (T)constructor.Invoke(parameters.ToArray());
        });
    }

    public IBuildScope Scope()
    {
        return new BuildScope(CopyOf(ServiceCollection));
    }

    private static IServiceCollection CopyOf(ServiceCollection serviceCollection)
    {
        return new ServiceCollection { serviceCollection };
    }
}