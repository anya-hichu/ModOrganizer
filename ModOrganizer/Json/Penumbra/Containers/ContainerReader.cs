using Dalamud.Plugin.Services;
using ModOrganizer.Json.Penumbra.Manipulations;
using ModOrganizer.Json.Readers;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Containers;

public class ContainerReader(IManipulationWrapperGenericReader manipulationWrapperGenericReader, IPluginLog pluginLog) : Reader<Container>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out Container? instance)
    {
        instance = null;

        if (!IsValue(element, JsonValueKind.Object)) return false;

        if (!TryGetOptionalDictValue(element, nameof(Container.Files), out var files)) return false;
        if (!TryGetOptionalDictValue(element, nameof(Container.FileSwaps), out var fileSwaps)) return false;

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
