﻿namespace Catears.EasyTestConstruct.ResolverFactories;

internal static class DefaultParameterResolverFactoryChain
{
    public static AggregateParameterResolverFactory FirstLink { get; } = new(new List<IParameterResolverFactory>()
    {
        new IntParameterResolverFactory(),
        new StringParameterResolverFactory(),
        new EnumParameterResolverFactory(),
        new DelegatingParameterResolverFactory()
    });
}