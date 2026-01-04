using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Containers;

public class NamedContainerReader(IReader<Container> containerReader, IPluginLog pluginLog) : Reader<NamedContainer>(pluginLog)
{
    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out NamedContainer? instance)
    {
        instance = null;

        if(!containerReader.TryRead(jsonElement, out var container))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Container)}] for [{nameof(NamedContainer)}]: {jsonElement}");
            return false;
        }

        string? name;
        switch (jsonElement.TryGetProperty(nameof(NamedContainer.Name), out var nameProperty))
        {
            case true when nameProperty.ValueKind == JsonValueKind.String:
                name = nameProperty.GetString();
                break;
            case true when nameProperty.ValueKind == JsonValueKind.Null:
            case false:
                name = null;
                break;
            default:
                PluginLog.Warning($"Expected property [{nameof(NamedContainer.Name)}] kind for [{nameof(NamedContainer)}] to be [{JsonValueKind.String}] or [{JsonValueKind.Null}] but found [{nameProperty.ValueKind}]: {jsonElement}");
                return false;
        };

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
