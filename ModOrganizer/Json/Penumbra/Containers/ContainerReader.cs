using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Json.Penumbra.Manipulations;
using ModOrganizer.Json.Penumbra.Options.Imcs;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Containers;

public class ContainerReader(IReader<ManipulationWrapper> manipulationWrapperReader, IPluginLog pluginLog) : Reader<Container>(pluginLog)
{
    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out Container? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        var files = new Dictionary<string, string>();
        if (jsonElement.TryGetProperty(nameof(Container.Files), out var filesProperty) && filesProperty.ValueKind != JsonValueKind.Null && !Assert.IsStringDict(filesProperty, out files))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(Container.Files)}] for [{nameof(Container)}]: {filesProperty}");
            return false;
        }

        var fileSwaps = new Dictionary<string, string>();
        if (jsonElement.TryGetProperty(nameof(Container.FileSwaps), out var fileSwapsProperty) && fileSwapsProperty.ValueKind != JsonValueKind.Null && !Assert.IsStringDict(fileSwapsProperty, out fileSwaps))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(Container.FileSwaps)}] for [{nameof(Container)}]: {fileSwapsProperty}");
            return false;
        }

        var manipulations = Array.Empty<ManipulationWrapper>();
        if (jsonElement.TryGetProperty(nameof(Container.Manipulations), out var manipulationsProperty) && manipulationsProperty.ValueKind != JsonValueKind.Null && !manipulationWrapperReader.TryReadMany(manipulationsProperty, out manipulations))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(Container.Manipulations)}] for [{nameof(Container)}]: {manipulationsProperty}");
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
