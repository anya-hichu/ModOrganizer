
using Microsoft.QualityTools.Testing.Fakes;

namespace ModOrganizer.Tests.ShimsContexts;

public abstract class ShimsContextBuilder : Builder<IDisposable>
{
    public event Action? OnShimsContext;

    public override IDisposable Build() 
    {
        var shimsContext = ShimsContext.Create();
        OnShimsContext?.Invoke();
        return shimsContext;
    }
}
