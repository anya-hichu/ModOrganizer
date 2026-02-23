using ModOrganizer.Json.Penumbra.Manipulations.Metas.Atchs.Entries;
using ModOrganizer.Json.Readers.Fakes;

namespace ModOrganizer.Tests.Json.Penumbra.Manipulations.Metas.Atchs.Entries;

public interface IStubbableMetaAtchEntryReader
{
    StubIReader<MetaAtchEntry> MetaAtchEntryReaderStub { get; }
}
