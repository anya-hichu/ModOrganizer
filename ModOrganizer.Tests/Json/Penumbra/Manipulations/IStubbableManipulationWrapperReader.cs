using ModOrganizer.Json.Penumbra.Manipulations;
using ModOrganizer.Json.Readers.Fakes;

namespace ModOrganizer.Tests.Json.Penumbra.Manipulations;

public interface IStubbableManipulationWrapperReader
{
    StubIReader<ManipulationWrapper> ManipulationWrapperReaderStub { get; }
}
