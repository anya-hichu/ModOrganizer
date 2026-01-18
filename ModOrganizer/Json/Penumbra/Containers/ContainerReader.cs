using Dalamud.Plugin.Services;
using ModOrganizer.Json.Penumbra.Manipulations;
using ModOrganizer.Json.Readers;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Containers;

public class ContainerReader(IManipulationWrapperGenericReader manipulationWrapperGenericReader, IPluginLog pluginLog) : Reader<Container>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out Container? instance)
    {
        instance = null;

        if (!IsValue(element, JsonValueKind.Object)) return false;

        var files = new Dictionary<string, string>();
        if (element.TryGetProperty(nameof(Container.Files), out var filesProperty) && filesProperty.ValueKind != JsonValueKind.Null && !IsDict(filesProperty, out files))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(Container.Files)}] for [{nameof(Container)}]: {element}");
            return false;
        }

        var fileSwaps = new Dictionary<string, string>();
        if (element.TryGetProperty(nameof(Container.FileSwaps), out var fileSwapsProperty) && fileSwapsProperty.ValueKind != JsonValueKind.Null && !IsDict(fileSwapsProperty, out fileSwaps))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(Container.FileSwaps)}] for [{nameof(Container)}]: {element}");
            return false;
        }

        var manipulations = Array.Empty<ManipulationWrapper>();
        if (element.TryGetProperty(nameof(Container.Manipulations), out var manipulationsProperty) && manipulationsProperty.ValueKind != JsonValueKind.Null && !manipulationWrapperGenericReader.TryReadMany(manipulationsProperty, out manipulations))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(Container.Manipulations)}] for [{nameof(Container)}]: {element}");
            return false;
        }

        instance = new()
        {
            Files = files,
            FileSwaps = fileSwaps,
            Manipulations = manipulations
        };

        return true;
    }
}
