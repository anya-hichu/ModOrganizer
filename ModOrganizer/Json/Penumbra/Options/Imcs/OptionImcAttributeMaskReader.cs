using Dalamud.Plugin.Services;
using ModOrganizer.Json.Asserts;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Options.Imcs;

public class OptionImcAttributeMaskReader(IAssert assert, IReader<Option> optionReader, IPluginLog pluginLog) : Reader<OptionImc>(assert, pluginLog), IOptionImcAttributeMaskReader
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out OptionImc? instance)
    {
        instance = null;

        if (!Assert.IsPropertyPresent(element, nameof(OptionImcAttributeMask.AttributeMask), out var optionAttributeMask)) return false;

        if (!optionReader.TryRead(element, out var option))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Option)}] for [{nameof(OptionImcAttributeMask)}]: {element}");
            return false;
        }

        instance = new OptionImcAttributeMask()
        {
            Name = option.Name,
            Description = option.Description,
            Priority = option.Priority,
            Image = option.Image,

            AttributeMask = optionAttributeMask.GetUInt16()
        };

        return true;
    }
}
