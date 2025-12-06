using Dalamud.Plugin.Services;
using ModOrganizer.Json.Containers;
using ModOrganizer.Json.Options;
using System;
using System.Diagnostics.CodeAnalysis;
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

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

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

        var options = Array.Empty<Option>();
        if (jsonElement.TryGetProperty(nameof(GroupCombining.Options), out var optionsProperty) && !OptionBuilder.TryBuildMany(optionsProperty, out options))
        {
            PluginLog.Warning($"Failed to build one or more [{nameof(OptionContainer)}] for [{nameof(GroupCombining)}]:\n\t{optionsProperty}");
            return false;
        }

        var containers = Array.Empty<NamedContainer>();
        if (jsonElement.TryGetProperty(nameof(GroupCombining.Containers), out var containersProperty) && !NamedContainerBuilder.TryBuildMany(containersProperty, out containers))
        {
            PluginLog.Warning($"Failed to build one or more [{nameof(NamedContainer)}] for [{nameof(GroupCombining)}]:\n\t{containersProperty}");
            return false;
        }

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
