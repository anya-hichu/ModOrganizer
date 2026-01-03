using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Penumbra.Containers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Options;

public class OptionContainerReader(IReader<Container> containerReader, IReader<Option> optionReader, IPluginLog pluginLog) : Reader<OptionContainer>(pluginLog)
{
    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out OptionContainer? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        if (!containerReader.TryRead(jsonElement, out var container))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Container)}] for [{nameof(OptionContainer)}]");
            return false;
        }

        if (!optionReader.TryRead(jsonElement, out var option))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Option)}] for [{nameof(OptionContainer)}]");
            return false;
        }

        instance = new()
        {
            Name = option.Name,
            Description = option.Description,
            Priority = option.Priority,
            Image = option.Image,

            Files = container.Files,
            FileSwaps = container.FileSwaps,
            Manipulations = container.Manipulations
        };

        return true;
    }
}
