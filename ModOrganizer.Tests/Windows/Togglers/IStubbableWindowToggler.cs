using ModOrganizer.Windows.Togglers.Fakes;

namespace ModOrganizer.Tests.Windows.Togglers;

public interface IStubbableWindowToggler
{
    StubIWindowToggler WindowTogglerStub { get; }
}
