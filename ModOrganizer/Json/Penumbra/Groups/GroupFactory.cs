using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers.Files;

namespace ModOrganizer.Json.Readers.Penumbra.Groups;

public class GroupFactory : TypeFactory<Group>, IReadableFile<Group>
{
    public FileReader FileReader { get; init; }

    public GroupFactory(IPluginLog pluginLog) : base(pluginLog)
    {
        FileReader = new(pluginLog);

        Readers.Add(GroupCombiningReader.TYPE, new GroupCombiningReader(pluginLog));
        Readers.Add(GroupMultiReader.TYPE, new GroupMultiReader(pluginLog));
        Readers.Add(GroupSingleReader.TYPE, new GroupSingleReader(pluginLog));
        Readers.Add(GroupImcReader.TYPE, new GroupImcReader(pluginLog));
    }
}
