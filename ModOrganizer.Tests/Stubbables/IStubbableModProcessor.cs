using ModOrganizer.Mods.Fakes;

namespace ModOrganizer.Tests.Stubbables;

public interface IStubbableModProcessor
{
    StubIModProcessor ModProcessorStub { get; }
}
