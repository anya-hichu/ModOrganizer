using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Containers;

public class NamedContainerReader(IPluginLog pluginLog) : Reader<NamedContainer>(pluginLog)
{
    private ContainerReader ContainerReader { get; set; } = new(pluginLog);

    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out NamedContainer? instance)
    {
        instance = null;

        if(!ContainerReader.TryRead(jsonElement, out var container))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Container)}] for [{nameof(NamedContainer)}]: {jsonElement}");
            return false;
        }

        var name = jsonElement.TryGetProperty(nameof(NamedContainer.Name), out var nameProperty) ? nameProperty.GetString() : null;

        instance = new()
        {
            Name = name,
            Files = container.Files,
            FileSwaps = container.FileSwaps,
            Manipulations = container.Manipulations
        };

        return true;
    }
}
