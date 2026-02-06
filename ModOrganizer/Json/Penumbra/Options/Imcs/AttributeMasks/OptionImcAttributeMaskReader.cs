using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Options.Imcs.AttributeMasks;

public class OptionImcAttributeMaskReader(IReader<Option> optionReader, IPluginLog pluginLog) : Reader<OptionImc>(pluginLog), IOptionImcAttributeMaskReader
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out OptionImc? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!optionReader.TryRead(element, out var option))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Option)}] for [{nameof(OptionImcAttributeMask)}]: {element}");
            return false;
        }

        if (!element.TryGetRequiredPropertyValue(nameof(OptionImcAttributeMask.AttributeMask), out ushort optionAttributeMask, PluginLog)) return false;

        instance = new OptionImcAttributeMask()
        {
            Name = option.Name,
            Description = option.Description,
            Priority = option.Priority,
            Image = option.Image,

            AttributeMask = optionAttributeMask
        };

        return true;
    }
}
