using Dalamud.Plugin.Services;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Penumbra.Groups.Bases;
using ModOrganizer.Json.Penumbra.Options;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Groups.Combinings;

public class GroupCombiningReader(IGroupBaseReader groupBaseReader, IReader<NamedContainer> namedContainerReader, IReader<Option> optionReader, IPluginLog pluginLog) : Reader<Group>(pluginLog)
{
    public static readonly string TYPE = "Combining";

    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out Group? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!groupBaseReader.TryRead(element, out var baseGroup))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Group)}] for [{nameof(GroupCombining)}]: {element}");
            return false;
        }

        if (baseGroup.Type != TYPE)
        {
            PluginLog.Warning($"Failed to read [{nameof(GroupCombining)}], invalid type [{baseGroup.Type}] (expected: {TYPE}): {element}");
            return false;
        }

        var options = Array.Empty<Option>();
        if (element.TryGetOptionalProperty(nameof(GroupCombining.Options), out var optionsProperty, PluginLog) && !optionReader.TryReadMany(optionsProperty, out options))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(Option)}] for [{nameof(GroupCombining)}]: {element}");
            return false;
        }

        var containers = Array.Empty<NamedContainer>();
        if (element.TryGetOptionalProperty(nameof(GroupCombining.Containers), out var containersProperty, PluginLog) && !namedContainerReader.TryReadMany(containersProperty, out containers))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(NamedContainer)}] for [{nameof(GroupCombining)}]: {element}");
            return false;
        }

        instance = new GroupCombining()
        {
            Name = baseGroup.Name,
            Type = baseGroup.Type,
            Description = baseGroup.Description,
            Image = baseGroup.Image,
            Priority = baseGroup.Priority,
            DefaultSettings = baseGroup.DefaultSettings,

            Options = options,
            Containers = containers
        };

        return true;
    }
}
