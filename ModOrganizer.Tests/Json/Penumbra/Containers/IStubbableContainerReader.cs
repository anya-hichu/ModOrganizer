using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Readers.Fakes;

namespace ModOrganizer.Tests.Json.Penumbra.Containers;

public interface IStubbableContainerReader
{
    StubIReader<Container> ContainerReaderStub { get; }
}
