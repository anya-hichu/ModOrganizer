using ModOrganizer.Json.Penumbra.Options;
using ModOrganizer.Json.Readers.Fakes;

namespace ModOrganizer.Tests.Json.Penumbra.Options;

public interface IStubbableOptionReader
{
    StubIReader<Option> OptionReaderStub { get; }
}
