using Dalamud.Plugin.Services;
using ModOrganizer.Json.Parsers;

namespace ModOrganizer.Json.Groups;

public class GroupFactory : TypeFactory<Group>, IFileParser<Group>
{
    public JsonParser JsonParser { get; init; }

    public GroupFactory(IPluginLog pluginLog) : base(pluginLog)
    {
        JsonParser = new(pluginLog);

        Builders.Add(GroupCombiningBuilder.TYPE, new GroupCombiningBuilder(pluginLog));
        Builders.Add(GroupMultiBuilder.TYPE, new GroupMultiBuilder(pluginLog));
        Builders.Add(GroupSingleBuilder.TYPE, new GroupSingleBuilder(pluginLog));
        Builders.Add(GroupImcBuilder.TYPE, new GroupImcBuilder(pluginLog));
    }
}
