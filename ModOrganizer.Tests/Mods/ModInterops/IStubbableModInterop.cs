using ModOrganizer.Mods.Fakes;

namespace ModOrganizer.Tests.Mods.ModInterops;

public interface IStubbableModInterop
{
    StubIModInterop ModInteropStub { get; }
}
