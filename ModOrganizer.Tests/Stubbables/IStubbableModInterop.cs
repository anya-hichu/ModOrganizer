using ModOrganizer.Mods.Fakes;

namespace ModOrganizer.Tests.Stubbables;

public interface IStubbableModInterop
{
    StubIModInterop ModInteropStub { get; }
}
