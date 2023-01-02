﻿using Catears.EasyConstruct.Providers;
using Catears.EasyConstruct.Registration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Catears.EasyConstruct;

public class BuildContext
{

    public class Options
    {
        public RegistrationMode RegistrationMode { get; set; } = RegistrationMode.Basic;

        public Action<BuildContext, Type> MockRegistrationMethod { get; set; } = (_, type) =>
        {
            InternalOptions.Default.MockRegistrationMethod(type);
        };

        public static Options Default { get; } = new();
    }

    internal record InternalOptions(RegistrationMode RegistrationMode, Action<Type> MockRegistrationMethod)
    {
        internal static InternalOptions Default { get; } = new(RegistrationMode.Basic, type =>
        {
            var msg = $"Trying to register abstract type or interface '{type.Name}' without any " +
                      $"defined function that handles such types. Add a `{nameof(Options.MockRegistrationMethod)}` " +
                      "when creating your build context to register mocks for these kinds of types when they " +
                      "are encountered.";
            throw new ArgumentException(msg);
        });
    }

    private ServiceCollection ServiceCollection { get; } = new();

    private InternalOptions BuildOptions { get; }

    public BuildContext(Options? options = null)
    {
        options ??= Options.Default;
        BuildOptions = new InternalOptions(
            options.RegistrationMode,
            type => options.MockRegistrationMethod(this, type));
        ServiceCollection.RegisterBasicValueProviders();
    }

    public void Register(Type type)
    {
        var registrator = new ServiceRegistrator(BuildOptions.MockRegistrationMethod);
        var dependencyWalker = GetDependencyWalkerForType(type);
        registrator.RegisterServicesOrThrow(ServiceCollection, dependencyWalker);
    }

    private IServiceDependencyWalker GetDependencyWalkerForType(Type type)
    {
        return BuildOptions.RegistrationMode == RegistrationMode.Recursive
            ? new RecursiveServiceDependencyWalker(type)
            : new BasicServiceDependencyWalker(type);
    }

    public void Register<T>() where T : class
    {
        Register(typeof(T));
    }

    public void Register(Type type, Func<IServiceProvider, object> builder)
    {
        ServiceCollection.AddTransient(type, builder);
    }

    public void Register<T>(Func<IServiceProvider, T> builder) where T : class
    {
        ServiceCollection.AddTransient(builder);
    }

    public void Register<T>(Func<T> builder) where T : class
    {
        Register(_ => builder());
    }

    public BuildScope Scope()
    {
        return new BuildScope(CopyOf(ServiceCollection));
    }

    private static IServiceCollection CopyOf(ServiceCollection serviceCollection)
    {
        return new ServiceCollection { serviceCollection };
    }
}