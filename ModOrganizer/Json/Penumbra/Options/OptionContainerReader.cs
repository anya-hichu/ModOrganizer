using Dalamud.Plugin.Services;
using ModOrganizer.Json.Penumbra.Containers;
using ModOrganizer.Json.Readers;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Options;

public class OptionContainerReader(IReader<Container> containerReader, IReader<Option> optionReader, IPluginLog pluginLog) : Reader<OptionContainer>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out OptionContainer? instance)
    {
        instance = null;

        if (!IsValue(element, JsonValueKind.Object)) return false;

        if (!containerReader.TryRead(element, out var container))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Container)}] for [{nameof(OptionContainer)}]: {element}");
            return false;
        }

        if (!optionReader.TryRead(element, out var option))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Option)}] for [{nameof(OptionContainer)}]: {element}");
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
