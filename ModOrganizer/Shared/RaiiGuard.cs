using System;
using System.Xml.Schema;

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
