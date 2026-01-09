using Microsoft.Extensions.DependencyInjection;
using System;

namespace ModOrganizer.Shared;

public abstract class CachedProvider : IDisposable
{
    private ServiceProvider? ServiceProviderCache { get; set; }

    protected abstract ServiceProvider BuildServiceProvider();

    private ServiceProvider GetServiceProvider()
    {
        if (ServiceProviderCache != null) return ServiceProviderCache;

        return ServiceProviderCache = BuildServiceProvider();
    }

    public void Dispose() => ServiceProviderCache?.Dispose();

    public T Get<T>() where T: notnull => GetServiceProvider().GetRequiredService<T>();
    public void Init<T>() where T : notnull => Get<T>();
}
