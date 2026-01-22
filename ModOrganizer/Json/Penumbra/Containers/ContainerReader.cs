using Dalamud.Plugin.Services;
using ModOrganizer.Json.Penumbra.Manipulations;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Containers;

public class ContainerReader(IManipulationWrapperGenericReader manipulationWrapperGenericReader, IPluginLog pluginLog) : Reader<Container>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out Container? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetOptionalPropertyValue(nameof(Container.Files), out Dictionary<string, string>? files, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(Container.FileSwaps), out Dictionary<string, string>? fileSwaps, PluginLog)) return false;

        ManipulationWrapper[]? manipulations = null;
        if (element.TryGetOptionalProperty(nameof(Container.Manipulations), out var manipulationsProperty, PluginLog) && !manipulationWrapperGenericReader.TryReadMany(manipulationsProperty, out manipulations))
        {
            PluginLog.Warning($"Failed to read one or more [Manipulations] for [Container]: {element}");
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
