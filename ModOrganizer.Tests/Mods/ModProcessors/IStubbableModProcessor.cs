using ModOrganizer.Mods.Fakes;

namespace ModOrganizer.Tests.Mods.ModProcessors;

public interface IStubbableModProcessor
{
    StubIModProcessor ModProcessorStub { get; }
}
