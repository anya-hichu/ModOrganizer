using Dalamud.Plugin.Services;
using ModOrganizer.Json.Manipulations;
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

        var manipulations = jsonElement.TryGetProperty(nameof(Container.Manipulations), out var manipulationProperty) ?
            manipulationProperty.EnumerateArray().SelectMany<JsonElement, ManipulationWrapper>(v => ManipulationWrapperFactory.TryBuild(v, out var manipulation) ? [manipulation] : []).ToArray() : [];

        instance = new()
        {
            Files = files,
            FileSwaps = fileSwaps,
            Manipulations = manipulations
        };

        return true;
    }
}
