using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Containers;

public class NamedContainerReader(IReader<Container> containerReader, IPluginLog pluginLog) : Reader<NamedContainer>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out NamedContainer? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!containerReader.TryRead(element, out var container))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Container)}] for [{nameof(NamedContainer)}]: {element}");
            return false;
        }

        if (!element.TryGetOptionalPropertyValue(nameof(NamedContainer.Name), out string? name, PluginLog)) return false;

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
