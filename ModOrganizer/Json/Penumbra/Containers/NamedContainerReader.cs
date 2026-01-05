using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Containers;

public class NamedContainerReader(IAssert assert, IReader<Container> containerReader, IPluginLog pluginLog) : Reader<NamedContainer>(assert, pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out NamedContainer? instance)
    {
        instance = null;

        if(!containerReader.TryRead(element, out var container))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Container)}] for [{nameof(NamedContainer)}]: {element}");
            return false;
        }

        if (!Assert.IsOptionalValue(element, nameof(NamedContainer.Name), out var name)) return false;

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
