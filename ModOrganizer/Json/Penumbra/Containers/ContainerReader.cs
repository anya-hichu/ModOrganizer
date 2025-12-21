using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Penumbra.Groups;
using ModOrganizer.Json.Readers.Penumbra.Manipulations;
using ModOrganizer.Json.Readers.Penumbra.Options.Imcs;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Penumbra.Containers;

public class ContainerReader(IPluginLog pluginLog) : Reader<Container>(pluginLog)
{
    private ManipulationWrapperFactory ManipulationWrapperFactory { get; init; } = new(pluginLog);

    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out Container? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        var files = new Dictionary<string, string>();
        if (jsonElement.TryGetProperty(nameof(Container.Files), out var filesProperty) && !Assert.IsStringDict(filesProperty, out files))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(Container.Files)}] for [{nameof(GroupImc)}]:\n\t{filesProperty}");
            return false;
        }

        var fileSwaps = new Dictionary<string, string>();
        if (jsonElement.TryGetProperty(nameof(Container.FileSwaps), out var fileSwapsProperty) && !Assert.IsStringDict(fileSwapsProperty, out fileSwaps))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(Container.FileSwaps)}] for [{nameof(GroupImc)}]:\n\t{fileSwapsProperty}");
            return false;
        }

        var manipulations = Array.Empty<ManipulationWrapper>();
        if (jsonElement.TryGetProperty(nameof(Container.Manipulations), out var manipulationsProperty) && !ManipulationWrapperFactory.TryReadMany(manipulationsProperty, out manipulations))
        {
            PluginLog.Warning($"Failed to read one or more [{nameof(OptionImc)}] for [{nameof(GroupImc)}]:\n\t{manipulationsProperty}");
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
