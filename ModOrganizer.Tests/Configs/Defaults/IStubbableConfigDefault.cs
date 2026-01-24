using ModOrganizer.Configs.Defaults.Fakes;

namespace ModOrganizer.Tests.Configs.Defaults;

public interface IStubbableConfigDefault
{
    StubIConfigDefault ConfigDefaultStub { get; }
}
