using Dalamud.Plugin.Services;
using ModOrganizer.Json.Penumbra.Groups.Bases;
using ModOrganizer.Json.Penumbra.Groups.Singles;
using ModOrganizer.Json.Penumbra.Options;
using ModOrganizer.Json.Penumbra.Options.Containers;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Groups;

public class GroupMultiReader(IGroupBaseReader groupBaseReader, IReader<OptionContainer> optionContainerReader, IPluginLog pluginLog) : Reader<Group>(pluginLog)
{
    public static readonly string TYPE = "Multi";

    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out Group? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!groupBaseReader.TryRead(element, out var baseGroup))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Group)}] for [{nameof(GroupMulti)}]: {element}");
            return false;
        }

        if (baseGroup.Type != TYPE)
        {
            PluginLog.Warning($"Failed to read [{nameof(GroupMulti)}], invalid type [{baseGroup.Type}] (expected: {TYPE}): {element}");
            return false;
        }

        var options = Array.Empty<OptionContainer>();
        if (element.TryGetProperty(nameof(GroupSingle.Options), out var optionsProperty) && !optionContainerReader.TryReadMany(optionsProperty, out options))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(OptionContainer)}] for [{nameof(GroupMulti)}]: {element}");
            return false;
        }

        if (!options.All(o => o.Priority.HasValue))
        {
            PluginLog.Warning($"Expected all [{nameof(OptionContainer)}] for [{nameof(GroupMulti)}] to have [{nameof(Option.Priority)}] value: {element}");
            return false;
        }

        instance = new GroupMulti()
        {
            Name = baseGroup.Name,
            Type = baseGroup.Type,
            Description = baseGroup.Description,
            Image = baseGroup.Image,
            Priority = baseGroup.Priority,
            DefaultSettings = baseGroup.DefaultSettings,

            Options = options
        };

        return true;
    }
}
