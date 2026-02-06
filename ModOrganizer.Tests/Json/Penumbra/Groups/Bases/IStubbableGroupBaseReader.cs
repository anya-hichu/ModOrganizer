using ModOrganizer.Json.Penumbra.Groups.Bases.Fakes;

namespace ModOrganizer.Tests.Json.Penumbra.Groups.Bases;

public interface IStubbableGroupBaseReader
{
    StubIGroupBaseReader GroupBaseReaderStub { get; }
}
