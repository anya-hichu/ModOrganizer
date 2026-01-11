using ModOrganizer.Mods.Fakes;

namespace ModOrganizer.Tests.Mods.Processors;

public interface IStubbableModProcessor
{
    StubIModProcessor ModProcessorStub { get; }
}
