using Dalamud.Plugin.Services;
using ModOrganizer.Json.Groups;
using ModOrganizer.Json.Manipulations;
using ModOrganizer.Json.Options.Imcs;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace ModOrganizer.Json.Containers;

public class ContainerBuilder(IPluginLog pluginLog) : Builder<Container>(pluginLog)
{
    private ManipulationWrapperFactory ManipulationWrapperFactory { get; init; } = new(pluginLog);

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out Container? instance)
    {
        instance = null;

        if (!Assert.IsObject(jsonElement)) return false;

        var files = jsonElement.TryGetProperty(nameof(Container.Files), out var filesProperty) ? 
            filesProperty.EnumerateObject().ToDictionary(p => p.Name, p => p.Value.GetString()!) : null;

        var fileSwaps = jsonElement.TryGetProperty(nameof(Container.FileSwaps), out var fileSwapsProperty) ? 
            fileSwapsProperty.EnumerateObject().ToDictionary(p => p.Name, p => p.Value.GetString()!) : null;

        var manipulations = Array.Empty<ManipulationWrapper>();
        if (jsonElement.TryGetProperty(nameof(Container.Manipulations), out var manipulationsProperty) && !ManipulationWrapperFactory.TryBuildMany(manipulationsProperty, out manipulations))
        {
            PluginLog.Warning($"Failed to build one of [{nameof(OptionImc)}] for [{nameof(GroupImc)}]:\n\t{manipulationsProperty}");
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
