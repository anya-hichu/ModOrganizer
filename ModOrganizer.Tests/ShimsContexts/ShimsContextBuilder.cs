
using Microsoft.QualityTools.Testing.Fakes;
using ModOrganizer.Shared;

namespace ModOrganizer.Tests.ShimsContexts;

public abstract class ShimsContextBuilder : IBuilder<IDisposable>, IShimmableContext
{
    public event Action? OnShimsContext;

    public IDisposable Build() 
    {
        var shimsContext = ShimsContext.Create();

        OnShimsContext?.Invoke();

        return shimsContext;
    }
}
