using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Files;

namespace ModOrganizer.Json.Penumbra.ModMetas;

public interface IModMetaReader : IReader<ModMetaV3>, IFileReader<ModMetaV3>
{
    // Empty
}
