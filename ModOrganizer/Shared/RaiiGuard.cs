using System;

namespace ModOrganizer.Shared;

public class RaiiGuard : IDisposable
{
    private Action Release { get; init; }

    public RaiiGuard(Action acquire, Action release)
    {
        Release = release;
        acquire();
    }

    public void Dispose() => Release();
}

public class RaiiGuard<T>(Func<T> acquire, Action<T> release) : IDisposable
{
    private Action<T> Release { get; init; } = release;
    public T Value { get; init; } = acquire();

    public void Dispose() => Release(Value);
}
