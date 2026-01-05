using Dalamud.Plugin.Services;
using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Json.Penumbra.Manipulations;
using ModOrganizer.Json.Penumbra.Options.Imcs;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Containers;

public class ContainerReader(IAssert assert, IReader<ManipulationWrapper> manipulationWrapperReader, IPluginLog pluginLog) : Reader<Container>(assert, pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out Container? instance)
    {
        instance = null;

        if (!Assert.IsValue(element, JsonValueKind.Object)) return false;

        var files = new Dictionary<string, string>();
        if (element.TryGetProperty(nameof(Container.Files), out var filesProperty) && filesProperty.ValueKind != JsonValueKind.Null && !Assert.IsStringDict(filesProperty, out files))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(Container.Files)}] for [{nameof(Container)}]: {element}");
            return false;
        }

        var fileSwaps = new Dictionary<string, string>();
        if (element.TryGetProperty(nameof(Container.FileSwaps), out var fileSwapsProperty) && fileSwapsProperty.ValueKind != JsonValueKind.Null && !Assert.IsStringDict(fileSwapsProperty, out fileSwaps))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(Container.FileSwaps)}] for [{nameof(Container)}]: {element}");
            return false;
        }

        var manipulations = Array.Empty<ManipulationWrapper>();
        if (element.TryGetProperty(nameof(Container.Manipulations), out var manipulationsProperty) && manipulationsProperty.ValueKind != JsonValueKind.Null && !manipulationWrapperReader.TryReadMany(manipulationsProperty, out manipulations))
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
