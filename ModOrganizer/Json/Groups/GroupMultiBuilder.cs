using Dalamud.Plugin.Services;
using ModOrganizer.Json.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Groups;

public class GroupMultiBuilder(IPluginLog pluginLog) : Builder<Group>(pluginLog)
{
    public static readonly string TYPE = "Multi";

    private GroupBuilder GroupBuilder { get; init; } = new(pluginLog);
    private OptionContainerBuilder OptionContainerBuilder { get; init; } = new(pluginLog);

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out Group? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        if (!GroupBuilder.TryBuild(jsonElement, out var group))
        {
            PluginLog.Debug($"Failed to build base [{nameof(Group)}] for [{nameof(GroupMulti)}]:\n\t{jsonElement}");
            return false;
        }

        if (group.Type != TYPE)
        {
            PluginLog.Warning($"Failed to build [{nameof(GroupMulti)}], invalid type [{group.Type}] (expected: {TYPE}):\n\t{jsonElement}");
            return false;
        }

        var options = Array.Empty<OptionContainer>();
        if (jsonElement.TryGetProperty(nameof(GroupSingle.Options), out var optionsProperty) && !OptionContainerBuilder.TryBuildMany(optionsProperty, out options))
        {
            PluginLog.Warning($"Failed to build one or more [{nameof(OptionContainer)}] for [{nameof(GroupMulti)}]:\n\t{optionsProperty}");
            return false;
        }

        instance = new GroupMulti()
        {
            Name = group.Name,
            Type = group.Type,
            Description = group.Description,
            Image = group.Image,
            Priority = group.Priority,
            DefaultSettings = group.DefaultSettings,

            Options = options
        };

        return true;
    }
}
