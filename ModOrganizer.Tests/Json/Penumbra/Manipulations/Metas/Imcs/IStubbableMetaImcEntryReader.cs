using ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs;
using ModOrganizer.Json.Readers.Fakes;

namespace ModOrganizer.Tests.Json.Penumbra.Manipulations.Metas.Imcs;

public interface IStubbableMetaImcEntryReader
{
    StubIReader<MetaImcEntry> MetaImcEntryReaderStub { get; }
}
