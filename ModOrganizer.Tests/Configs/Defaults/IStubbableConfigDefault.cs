using ModOrganizer.Configs.Fakes;

namespace ModOrganizer.Tests.Configs.Defaults;

public interface IStubbableConfigDefault
{
    StubIConfigDefault ConfigDefaultStub { get; }
}
