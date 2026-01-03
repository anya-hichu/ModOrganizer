using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Files;

namespace ModOrganizer.Json.Penumbra.Groups;

public interface IGroupReaderFactory : IReader<Group>, IReadableFromFile<Group>
{
    // Empty
}
