using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Penumbra.Options;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Penumbra.Options.Imcs;

public class OptionImcIsDisableSubModReader(IPluginLog pluginLog) : Reader<OptionImc>(pluginLog)
{
    private OptionReader OptionReader { get; init; } = new(pluginLog);

    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out OptionImc? instance)
    {
        instance = null;

        if (!Assert.IsPropertyPresent(jsonElement, nameof(OptionImcIsDisableSubMod.IsDisableSubMod), out var optionIsDisableSubMod)) return false;

        if (!OptionReader.TryRead(jsonElement, out var option))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Option)}] for [{nameof(OptionImcIsDisableSubMod)}]");
            return false;
        }

        instance = new OptionImcIsDisableSubMod()
        {
            Name = option.Name,
            Description = option.Description,
            Priority = option.Priority,
            Image = option.Image,

            IsDisableSubMod = optionIsDisableSubMod.GetBoolean()
        };

        return true;
    }
}
