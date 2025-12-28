using ModOrganizer.Shared.Fakes;

namespace ModOrganizer.Tests.Shared.Clock;

public interface IStubbableClock
{
    StubIClock ClockStub { get; }
}
