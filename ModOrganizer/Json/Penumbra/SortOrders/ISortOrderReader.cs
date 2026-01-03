using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Files;

namespace ModOrganizer.Json.Penumbra.SortOrders;

public interface ISortOrderReader : IReader<SortOrder>, IReadableFromFile<SortOrder>
{
    // Empty
}
