using ModOrganizer.Commands.Fakes;

namespace ModOrganizer.Tests.Commands.Printers;

public interface IStubbableCommandPrinter
{
    StubICommandPrinter CommandPrinterStub { get; }
}
