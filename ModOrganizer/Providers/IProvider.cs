using System;

namespace ModOrganizer.Providers;

public interface IProvider
{
    T Get<T>() where T : notnull;
    void Init<T>() where T : notnull;
}
