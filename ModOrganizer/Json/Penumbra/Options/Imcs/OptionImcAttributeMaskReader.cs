using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Penumbra.Options;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Options.Imcs;

public class OptionImcAttributeMaskReader(IPluginLog pluginLog) : Reader<OptionImc>(pluginLog)
{
    private OptionReader OptionReader { get; init; } = new(pluginLog);

    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out OptionImc? instance)
    {
        instance = null;

        if (!Assert.IsPropertyPresent(jsonElement, nameof(OptionImcAttributeMask.AttributeMask), out var optionAttributeMask)) return false;

        if (!OptionReader.TryRead(jsonElement, out var option))
        {
            PluginLog.Debug($"Failed to read base [{nameof(Option)}] for [{nameof(OptionImcAttributeMask)}]:\n\t{jsonElement}");
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
