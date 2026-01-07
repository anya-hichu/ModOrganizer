using Microsoft.Extensions.DependencyInjection;
using System;

namespace ModOrganizer.Providers;

public abstract class CachedProvider : Provider, IDisposable
{
    private ServiceProvider? ServiceProviderCache { get; set; }

    protected abstract ServiceProvider BuildServiceProvider();

    protected override ServiceProvider GetServiceProvider()
    {
        if (ServiceProviderCache != null) return ServiceProviderCache;

        return ServiceProviderCache = BuildServiceProvider();
    }

    public void Dispose() => ServiceProviderCache?.Dispose();
}
