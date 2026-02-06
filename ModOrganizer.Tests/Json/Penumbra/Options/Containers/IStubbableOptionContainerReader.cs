using ModOrganizer.Json.Penumbra.Options.Containers;
using ModOrganizer.Json.Readers.Fakes;

namespace ModOrganizer.Tests.Json.Penumbra.Options.Containers;

public interface IStubbableOptionContainerReader
{
    StubIReader<OptionContainer> OptionContainerStub { get; }
}
