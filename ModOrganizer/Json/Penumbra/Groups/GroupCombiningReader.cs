using Dalamud.Plugin.Services;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Penumbra.Options;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Groups;

public class GroupCombiningReader(IAssert assert, IGroupBaseReader groupBaseReader, IReader<NamedContainer> namedContainerReader, IReader<Option> optionReader, IPluginLog pluginLog) : Reader<Group>(assert, pluginLog)
{
    public static readonly string TYPE = "Combining";

    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out Group? instance)
    {
        instance = null;

        if (!Assert.IsValue(element, JsonValueKind.Object)) return false;

        if (!groupBaseReader.TryRead(element, out var group))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Group)}] for [{nameof(GroupCombining)}]: {element}");
            return false;
        }

        if (group.Type != TYPE)
        {
            PluginLog.Warning($"Failed to read [{nameof(GroupCombining)}], invalid type [{group.Type}] (expected: {TYPE}): {element}");
            return false;
        }

        var options = Array.Empty<Option>();
        if (element.TryGetProperty(nameof(GroupCombining.Options), out var optionsProperty) && !optionReader.TryReadMany(optionsProperty, out options))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(OptionContainer)}] for [{nameof(GroupCombining)}]: {element}");
            return false;
        }

        var containers = Array.Empty<NamedContainer>();
        if (element.TryGetProperty(nameof(GroupCombining.Containers), out var containersProperty) && !namedContainerReader.TryReadMany(containersProperty, out containers))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(NamedContainer)}] for [{nameof(GroupCombining)}]: {element}");
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
