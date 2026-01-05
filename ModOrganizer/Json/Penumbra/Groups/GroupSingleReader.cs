using Dalamud.Plugin.Services;
using ModOrganizer.Json.Penumbra.Options;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Groups;

public class GroupSingleReader(IAssert assert, IReader<Group> groupReader, IReader<OptionContainer> optionContainerReader, IPluginLog pluginLog) : Reader<Group>(assert, pluginLog)
{
    public static readonly string TYPE = "Single";

    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out Group? instance)
    {
        instance = null;

        if (!Assert.IsValue(element, JsonValueKind.Object)) return false;

        if (!groupReader.TryRead(element, out var group))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Group)}] for [{nameof(GroupSingle)}]: {element}");
            return false;
        }

        if (group.Type != TYPE)
        {
            PluginLog.Warning($"Failed to read [{nameof(GroupSingle)}], invalid type [{group.Type}] (expected: {TYPE}): {element}");
            return false;
        }

        var options = Array.Empty<OptionContainer>();
        if (element.TryGetProperty(nameof(GroupSingle.Options), out var optionsProperty) && !optionContainerReader.TryReadMany(optionsProperty, out options))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(OptionContainer)}] for [{nameof(GroupSingle)}]: {optionsProperty}");
            return false;
        }

        instance = new GroupSingle()
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
