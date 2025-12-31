namespace ModOrganizer.Tests.ShimsContexts;

public interface IShimmableContext
{
    IDisposable Context { get; }
}
