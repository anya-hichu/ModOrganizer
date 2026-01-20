using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Options.Imcs;

public class OptionImcIsDisableSubModReader(IReader<Option> optionReader, IPluginLog pluginLog) : Reader<OptionImc>(pluginLog), IOptionImcIsDisableSubModReader
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out OptionImc? instance)
    {
        instance = null;

        if (!element.TryGetRequiredPropertyValue(nameof(OptionImcIsDisableSubMod.IsDisableSubMod), out bool optionIsDisableSubMod, PluginLog)) return false;

        if (!optionReader.TryRead(element, out var option))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Option)}] for [{nameof(OptionImcIsDisableSubMod)}]: {element}");
            return false;
        }

        instance = new OptionImcIsDisableSubMod()
        {
            Name = option.Name,
            Description = option.Description,
            Priority = option.Priority,
            Image = option.Image,

            IsDisableSubMod = optionIsDisableSubMod
        };

        return true;
    }
}
