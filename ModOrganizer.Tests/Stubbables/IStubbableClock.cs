using ModOrganizer.Shared.Fakes;

namespace ModOrganizer.Tests.Stubbables;

public interface IStubbableClock
{
    StubIClock ClockStub { get; }
}
