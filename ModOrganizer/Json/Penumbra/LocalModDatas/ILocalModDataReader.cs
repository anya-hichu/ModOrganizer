using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Files;

namespace ModOrganizer.Json.Penumbra.LocalModDatas;

public interface ILocalModDataReader : IReader<LocalModData>, IFileReader<LocalModData>
{
    // Empty
}
