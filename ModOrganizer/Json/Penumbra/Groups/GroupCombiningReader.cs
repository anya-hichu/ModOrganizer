using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Penumbra.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Groups;

public class GroupCombiningReader(IReader<Group> groupReader, IReader<NamedContainer> namedContainerReader, IReader<Option> optionReader, IPluginLog pluginLog) : Reader<Group>(pluginLog)
{
    public static readonly string TYPE = "Combining";

    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out Group? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        if (!groupReader.TryRead(jsonElement, out var group))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Group)}] for [{nameof(GroupCombining)}]: {jsonElement}");
            return false;
        }

        if (group.Type != TYPE)
        {
            PluginLog.Warning($"Failed to read [{nameof(GroupCombining)}], invalid type [{group.Type}] (expected: {TYPE}): {jsonElement}");
            return false;
        }

        var options = Array.Empty<Option>();
        if (jsonElement.TryGetProperty(nameof(GroupCombining.Options), out var optionsProperty) && !optionReader.TryReadMany(optionsProperty, out options))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(OptionContainer)}] for [{nameof(GroupCombining)}]: {optionsProperty}");
            return false;
        }

        var containers = Array.Empty<NamedContainer>();
        if (jsonElement.TryGetProperty(nameof(GroupCombining.Containers), out var containersProperty) && !namedContainerReader.TryReadMany(containersProperty, out containers))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(NamedContainer)}] for [{nameof(GroupCombining)}]: {containersProperty}");
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
