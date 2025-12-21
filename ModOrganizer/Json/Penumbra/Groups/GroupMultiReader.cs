using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Penumbra.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Penumbra.Groups;

public class GroupMultiReader(IPluginLog pluginLog) : Reader<Group>(pluginLog)
{
    public static readonly string TYPE = "Multi";

    private GroupReader GroupReader { get; init; } = new(pluginLog);
    private OptionContainerReader OptionContainerReader { get; init; } = new(pluginLog);

    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out Group? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        if (!GroupReader.TryRead(jsonElement, out var group))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Group)}] for [{nameof(GroupMulti)}]:\n\t{jsonElement}");
            return false;
        }

        if (group.Type != TYPE)
        {
            PluginLog.Warning($"Failed to read [{nameof(GroupMulti)}], invalid type [{group.Type}] (expected: {TYPE}):\n\t{jsonElement}");
            return false;
        }

        var options = Array.Empty<OptionContainer>();
        if (jsonElement.TryGetProperty(nameof(GroupSingle.Options), out var optionsProperty) && !OptionContainerReader.TryReadMany(optionsProperty, out options))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(OptionContainer)}] for [{nameof(GroupMulti)}]:\n\t{optionsProperty}");
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
