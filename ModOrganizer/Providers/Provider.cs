using Microsoft.Extensions.DependencyInjection;

namespace ModOrganizer.Providers;

public abstract class Provider : IProvider
{
    protected abstract ServiceProvider GetServiceProvider();

    public T Get<T>() where T: notnull => GetServiceProvider().GetRequiredService<T>();
    public void Init<T>() where T : notnull => Get<T>();
}
