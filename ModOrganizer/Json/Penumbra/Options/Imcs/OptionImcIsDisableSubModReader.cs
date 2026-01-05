using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Options.Imcs;

public class OptionImcIsDisableSubModReader(IAssert assert, IReader<Option> optionReader, IPluginLog pluginLog) : Reader<OptionImc>(assert, pluginLog), IOptionImcIsDisableSubModReader
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out OptionImc? instance)
    {
        instance = null;

        if (!Assert.IsPropertyPresent(element, nameof(OptionImcIsDisableSubMod.IsDisableSubMod), out var optionIsDisableSubMod)) return false;

        if (!optionReader.TryRead(element, out var option))
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
