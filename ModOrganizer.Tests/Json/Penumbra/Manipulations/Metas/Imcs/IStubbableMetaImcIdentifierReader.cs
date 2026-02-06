using ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs.Identifiers;
using ModOrganizer.Json.Readers.Fakes;

namespace ModOrganizer.Tests.Json.Penumbra.Manipulations.Metas.Imcs;

public interface IStubbableMetaImcIdentifierReader
{
    StubIReader<MetaImcIdentifier> MetaImcIdentifierReaderStub { get; }
}
