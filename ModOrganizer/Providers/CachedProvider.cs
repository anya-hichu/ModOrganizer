using Microsoft.Extensions.DependencyInjection;
using System;

namespace ModOrganizer.Providers;

public abstract class CachedProvider : IDisposable
{
    private ServiceProvider? MaybeServiceProviderCache { get; set; }

    protected abstract ServiceProvider BuildServiceProvider();

    private ServiceProvider GetServiceProvider()
    {
        if (MaybeServiceProviderCache != null) return MaybeServiceProviderCache;

        return MaybeServiceProviderCache = BuildServiceProvider();
    }

    public void Dispose() => MaybeServiceProviderCache?.Dispose();

    public T Get<T>() where T: notnull => GetServiceProvider().GetRequiredService<T>();
    public void Init<T>() where T : notnull => Get<T>();
}
