using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Clipboards;
using ModOrganizer.Json.Readers.Files;

namespace ModOrganizer.Json.ConfigDatas.Readers;

public interface IConfigDataReader : IReader<ConfigData>, IClipboardReader<ConfigData>, IFileReader<ConfigData>
{
    // Empty
}
