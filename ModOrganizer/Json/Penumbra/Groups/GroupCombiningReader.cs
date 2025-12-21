using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Penumbra.Containers;
using ModOrganizer.Json.Readers.Penumbra.Options;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Penumbra.Groups;

public class GroupCombiningReader(IPluginLog pluginLog) : Reader<Group>(pluginLog)
{
    public static readonly string TYPE = "Combining";

    private GroupReader GroupReader { get; init; } = new(pluginLog);
    private NamedContainerReader NamedContainerReader { get; init; } = new(pluginLog);
    private OptionReader OptionReader { get; init; } = new(pluginLog);

    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out Group? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        if (!GroupReader.TryRead(jsonElement, out var group))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Group)}] for [{nameof(GroupCombining)}]:\n\t{jsonElement}");
            return false;
        }

        if (group.Type != TYPE)
        {
            PluginLog.Warning($"Failed to read [{nameof(GroupCombining)}], invalid type [{group.Type}] (expected: {TYPE}):\n\t{jsonElement}");
            return false;
        }

        var options = Array.Empty<Option>();
        if (jsonElement.TryGetProperty(nameof(GroupCombining.Options), out var optionsProperty) && !OptionReader.TryReadMany(optionsProperty, out options))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(OptionContainer)}] for [{nameof(GroupCombining)}]:\n\t{optionsProperty}");
            return false;
        }

        var containers = Array.Empty<NamedContainer>();
        if (jsonElement.TryGetProperty(nameof(GroupCombining.Containers), out var containersProperty) && !NamedContainerReader.TryReadMany(containersProperty, out containers))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(NamedContainer)}] for [{nameof(GroupCombining)}]:\n\t{containersProperty}");
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
