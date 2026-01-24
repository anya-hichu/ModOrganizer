using System;

namespace ModOrganizer.Providers;

public interface ICachedProvider : IDisposable
{
    T Get<T>() where T : notnull;
    void Init<T>() where T : notnull;
}
