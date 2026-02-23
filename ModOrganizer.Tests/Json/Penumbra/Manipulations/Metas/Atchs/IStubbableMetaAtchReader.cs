using ModOrganizer.Json.Penumbra.Manipulations.Metas.Atchs;
using ModOrganizer.Json.Readers.Fakes;

namespace ModOrganizer.Tests.Json.Penumbra.Manipulations.Metas.Atchs;

public interface IStubbableMetaAtchReader
{
    StubIReader<MetaAtch> MetaAtchReaderStub { get; }
}
