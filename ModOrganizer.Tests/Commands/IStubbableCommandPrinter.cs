using ModOrganizer.Commands.Fakes;

namespace ModOrganizer.Tests.Commands;

public interface IStubbableCommandPrinter
{
    StubICommandPrinter CommandPrinterStub { get; }
}
