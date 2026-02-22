using ModOrganizer.Json.Writers;
using ModOrganizer.Json.Writers.Clipboards;
using ModOrganizer.Json.Writers.Files;

namespace ModOrganizer.Json.ConfigDatas;

public interface IConfigDataWriter : IWriter<ConfigData>, IClipboardWriter<ConfigData>, IFileWriter<ConfigData>
{
    // Empty
}
