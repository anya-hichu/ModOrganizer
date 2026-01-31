using ModOrganizer.Json.Penumbra.Groups.Fakes;

namespace ModOrganizer.Tests.Json.Penumbra.Groups.Bases;

public interface IStubbableGroupBaseReader
{
    StubIGroupBaseReader GroupBaseReaderStub { get; }
}
