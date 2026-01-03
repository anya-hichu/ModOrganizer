using ModOrganizer.Json.Readers.Elements.Fakes;

namespace ModOrganizer.Tests.Json.Readers.Elements;

public interface IStubbableElementReader
{
    StubIElementReader ElementReaderStub { get; }
}
