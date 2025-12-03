using Dalamud.Plugin.Services;
using ModOrganizer.Json.Files;

namespace ModOrganizer.Json.Groups;

public class GroupFactory : TypeFactory<Group>, IFileBuilder<Group>
{
    public Parser Parser { get; init; }

    public GroupFactory(IPluginLog pluginLog) : base(pluginLog)
    {
        Parser = new(pluginLog);

        Builders.Add(GroupCombiningBuilder.TYPE, new GroupCombiningBuilder(pluginLog));
        Builders.Add(GroupMultiBuilder.TYPE, new GroupMultiBuilder(pluginLog));
        Builders.Add(GroupSingleBuilder.TYPE, new GroupSingleBuilder(pluginLog));
        Builders.Add(GroupImcBuilder.TYPE, new GroupImcBuilder(pluginLog));
    }
}
