using Dalamud.Plugin.Services;
using ModOrganizer.Json.Loaders;

namespace ModOrganizer.Json.Groups;

public class GroupFactory : Factory<Group>, IFileLoader<Group>
{
    public JsonParser JsonParser { get; init; }

    public GroupFactory(IPluginLog pluginLog) : base(pluginLog)
    {
        JsonParser = new(pluginLog);

        Builders.Add(GroupCombiningBuilder.TYPE, new GroupCombiningBuilder(pluginLog));
        Builders.Add(GroupSingleBuilder.TYPE, new GroupSingleBuilder(pluginLog));
    }
}
