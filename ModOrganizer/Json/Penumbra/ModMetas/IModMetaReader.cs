using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Files;

namespace ModOrganizer.Json.Penumbra.ModMetas;

public interface IModMetaReader : IReader<ModMeta>, IFileReader<ModMeta>
{
    // Empty
}
