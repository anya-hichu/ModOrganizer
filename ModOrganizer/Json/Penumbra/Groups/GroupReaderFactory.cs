using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Files;

namespace ModOrganizer.Json.Penumbra.Groups;

public class GroupReaderFactory : TypeReaderFactory<Group>, IReadableFile<Group>
{
    public FileReader FileReader { get; init; }

    public GroupReaderFactory(IPluginLog pluginLog) : base(pluginLog)
    {
        FileReader = new(pluginLog);

        Readers.Add(GroupCombiningReader.TYPE, new GroupCombiningReader(pluginLog));
        Readers.Add(GroupMultiReader.TYPE, new GroupMultiReader(pluginLog));
        Readers.Add(GroupSingleReader.TYPE, new GroupSingleReader(pluginLog));
        Readers.Add(GroupImcReader.TYPE, new GroupImcReader(pluginLog));
    }
}
