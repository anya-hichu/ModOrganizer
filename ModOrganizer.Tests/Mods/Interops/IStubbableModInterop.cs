using ModOrganizer.Mods.Fakes;

namespace ModOrganizer.Tests.Mods.Interops;

public interface IStubbableModInterop
{
    StubIModInterop ModInteropStub { get; }
}
