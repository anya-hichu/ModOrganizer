using Dalamud.Plugin.Services;
using ModOrganizer.Json.Containers;
using ModOrganizer.Json.Options;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace ModOrganizer.Json.Groups;

public class GroupCombiningBuilder(IPluginLog pluginLog) : Builder<Group>(pluginLog)
{
    public static readonly string TYPE = "Combining";

    private GroupBuilder GroupBuilder { get; init; } = new(pluginLog);
    private NamedContainerBuilder NamedContainerBuilder { get; init; } = new(pluginLog);
    private OptionBuilder OptionBuilder { get; init; } = new(pluginLog);

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out Group? instance)
    {
        instance = null;

        if (!AssertObject(jsonElement)) return false;

        if (!GroupBuilder.TryBuild(jsonElement, out var group))
        {
            PluginLog.Debug($"Failed to build base [{nameof(Group)}] for [{nameof(GroupCombining)}]:\n\t{jsonElement}");
            return false;
        }

        if (group.Type != TYPE)
        {
            PluginLog.Warning($"Failed to build [{nameof(GroupCombining)}], invalid type [{group.Type}] (expected: {TYPE}):\n\t{jsonElement}");
            return false;
        }

        // Provide default values to make it easier to use
        var options = jsonElement.TryGetProperty(nameof(GroupCombining.Options), out var optionsProperty) ?
            optionsProperty.EnumerateArray().SelectMany<JsonElement, Option>(j => OptionBuilder.TryBuild(j, out var option) ? [option] : []).ToArray() : [];

        var containers = jsonElement.TryGetProperty(nameof(GroupCombining.Containers), out var containersProperty) ?
            containersProperty.EnumerateArray().SelectMany<JsonElement, NamedContainer>(j => NamedContainerBuilder.TryBuild(j, out var namedContainer) ? [namedContainer] : []).ToArray() : [];

        instance = new GroupCombining()
        {
            Name = group.Name,
            Type = group.Type,
            Description = group.Description,
            Image = group.Image,
            Priority = group.Priority,
            DefaultSettings = group.DefaultSettings,

            Options = options,
            Containers = containers
        };

        return true;
    }
}
