using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Files;

namespace ModOrganizer.Json.Penumbra.DefaultMods;

public interface IDefaultModReader : IReader<DefaultMod>, IFileReader<DefaultMod>
{
    // Empty
}
