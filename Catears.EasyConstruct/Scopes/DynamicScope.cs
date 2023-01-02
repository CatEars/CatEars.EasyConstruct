﻿using Catears.EasyConstruct.Registration;
using Microsoft.Extensions.DependencyInjection;

namespace Catears.EasyConstruct.Scopes;

internal class DynamicScope : BuildScope
{
    private Func<Type, object>? MockFactoryMethod { get; }

    private SingleEncounterDependencyListerDecorator EncounterLister { get; }

    private IDependencyLister DependencyLister { get; }
    
    public DynamicScope(IServiceCollection serviceCollection,
        Func<Type, object>? mockFactoryMethod) : base(serviceCollection)
    {
        MockFactoryMethod = mockFactoryMethod;
        var registeredServices = serviceCollection.Select(descriptor => descriptor.ServiceType);
        var encounterLister = new SingleEncounterDependencyListerDecorator(
            new ConstructorParameterDependencyLister(),
            registeredServices.ToHashSet()
        );
        EncounterLister = encounterLister;
        DependencyLister = new RecursiveDependencyListerDecorator(EncounterLister);
    }

    protected override object InternalResolve(Type type)
    {
        EnsureDependencyTreeExists(type);
        return base.InternalResolve(type);
    }

    protected override void InternalMemoize(Type type)
    {
        EnsureDependencyTreeExists(type);
        base.InternalMemoize(type);
    }

    private void EnsureDependencyTreeExists(Type type)
    {
        if (EncounterLister.HasEncounteredType(type))
        {
            return;
        }

        AddDependencyTreeToCollection(type);
        InvalidateCurrentProvider();
    }

    private void AddDependencyTreeToCollection(Type type)
    {
        var registrator = new ServiceRegistrator(MockFactoryMethod);
        registrator.RegisterServicesOrThrow(Collection, DependencyLister, type);
    }
}