
using Microsoft.QualityTools.Testing.Fakes;

namespace ModOrganizer.Tests.ShimsContexts;

public abstract class ShimsContextBuilder : Builder<IDisposable>
{
    public IDisposable Context { get; init; } = ShimsContext.Create();

    public override IDisposable Build() => Context;
}
