using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Penumbra.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Groups;

public class GroupMultiReader(IReader<Group> groupReader, IReader<OptionContainer> optionContainerReader, IPluginLog pluginLog) : Reader<Group>(pluginLog)
{
    public static readonly string TYPE = "Multi";

    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out Group? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        if (!groupReader.TryRead(jsonElement, out var group))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Group)}] for [{nameof(GroupMulti)}]: {jsonElement}");
            return false;
        }

        if (group.Type != TYPE)
        {
            PluginLog.Warning($"Failed to read [{nameof(GroupMulti)}], invalid type [{group.Type}] (expected: {TYPE}): {jsonElement}");
            return false;
        }

        var options = Array.Empty<OptionContainer>();
        if (jsonElement.TryGetProperty(nameof(GroupSingle.Options), out var optionsProperty) && !optionContainerReader.TryReadMany(optionsProperty, out options))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(OptionContainer)}] for [{nameof(GroupMulti)}]: {optionsProperty}");
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
