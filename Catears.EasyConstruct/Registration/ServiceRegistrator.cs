﻿using System.Reflection;
using Catears.EasyConstruct.Resolvers;
using Microsoft.Extensions.DependencyInjection;

namespace Catears.EasyConstruct.Registration;

internal class ServiceRegistrator
{
    private Action<Type> MockRegistrationMethod { get; }

    public ServiceRegistrator(Action<Type> mockRegistrationMethod)
    {
        MockRegistrationMethod = mockRegistrationMethod;
    }

    internal void RegisterServicesOrThrow(
        IServiceCollection collection,
        IServiceDependencyWalker walker)
    {
        foreach (var dependency in walker.ListDependencies())
        {
            RegisterServiceOrThrow(collection, dependency);
        }
    }

    internal void RegisterServiceOrThrow(IServiceCollection collection, ServiceRegistrationContext context)
    {
        if (context.IsOpenGenericType)
        {
            // Services that are open and generic (like `typeof(MyTypeWithUnspecifiedParameters<>)`
            // will only be constructed if they have a specific implementation type. The object will not be built
            // with a factory method.
            collection.AddTransient(context.ServiceToRegister, context.ServiceToRegister);
            return;
        }

        if (context.IsBasicType)
        {
            return;
        }

        if (context.IsMockIntendedType)
        {
            MockRegistrationMethod(context.ServiceToRegister);
            return;
        }

        var constructorToRegister = FindAppropriateConstructorOrThrow(context);
        Register(collection, context, constructorToRegister);
    }

    internal static ConstructorInfo FindAppropriateConstructorOrThrow(ServiceRegistrationContext context)
    {
        // Most user-defined objects in C# will have a constructor. For most classes and records this is automatically generated.
        // However, for static classes and structs they are not. In those cases you should not be able to register
        // without a builder function so we do not allow automatic service registration for types without a constructor.
        // Only 1+ constructors.
        if (context.Constructors.Length == 1)
        {
            return context.Constructors.First();
        }

        bool IsPreferredConstructor(ConstructorInfo info) =>
            Attribute.IsDefined(info, typeof(PreferredConstructorAttribute));

        var markedConstructor = context.Constructors.FirstOrDefault(IsPreferredConstructor);
        if (markedConstructor != null)
        {
            return markedConstructor;
        }

        var msg = $"Constructor for type {context.ServiceToRegister.Name} did not contain exactly 1 constructor, " +
                  $"and it did not contain a constructor marked as {nameof(PreferredConstructorAttribute)} and can " +
                  $"therefore not be registered for automatically constructing";
        throw new ArgumentException(msg);
    }

    private static void Register(IServiceCollection serviceCollection, ServiceRegistrationContext context,
        ConstructorInfo constructor)
    {
        var parameterDescriptors = constructor.GetParameters();
        var sortedByPosition = parameterDescriptors.OrderBy(paramInfo => paramInfo.Position);
        var parameterResolvers = sortedByPosition
            .Select(ParameterResolverCollection.GetResolverForType)
            .ToList();

        serviceCollection.AddTransient(context.ServiceToRegister, services =>
        {
            var parameters = parameterResolvers.Select(resolver => resolver.ResolveParameter(services));
            return constructor.Invoke(parameters.ToArray());
        });
    }
}