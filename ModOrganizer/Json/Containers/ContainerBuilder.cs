using Dalamud.Plugin.Services;
using ModOrganizer.Json.Manipulations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;

namespace ModOrganizer.Json.Containers;

public class ContainerBuilder<T>(ManipulationFactory manipulationFactory, IPluginLog pluginLog) : Builder<T>(pluginLog) where T : Container, new()
{
    private ManipulationFactory ManipulationFactory { get; init; } = manipulationFactory;

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out T? instance)
    {
        instance = default;

        if (jsonElement.ValueKind != JsonValueKind.Object)
        {
            PluginLog.Warning($"Failed to build [{nameof(Container)}], expected root object but got [{jsonElement.ValueKind}]");
            return false;
        }

        var files = jsonElement.TryGetProperty(nameof(Container.Files), out var filesProperty) ? 
            filesProperty.EnumerateObject().ToDictionary(p => p.Name, p => p.Value.GetString()!) : null;

        var fileSwaps = jsonElement.TryGetProperty(nameof(Container.FileSwaps), out var fileSwapsProperty) ? 
            fileSwapsProperty.EnumerateObject().ToDictionary(p => p.Name, p => p.Value.GetString()!) : null;

        var manipulations = jsonElement.TryGetProperty(nameof(Container.Manipulations), out var manipulationProperty) ?
            manipulationProperty.EnumerateArray().SelectMany<JsonElement, ManipulationWrapper>(v => ManipulationFactory.TryBuild(v, out var manipulation) ? [manipulation] : []).ToArray() : [];

        instance = new T()
        {
            Files = files,
            FileSwaps = fileSwaps,
            Manipulations = manipulations
        };

        return true;
    }
}
