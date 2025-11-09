using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Options.Imcs;

public class OptionImcAttributeMaskBuilder(IPluginLog pluginLog) : Builder<OptionImc>(pluginLog)
{
    private OptionBuilder OptionBuilder { get; init; } = new(pluginLog);

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out OptionImc? instance)
    {
        instance = default;

        if (!jsonElement.TryGetProperty(nameof(OptionImcAttributeMask.AttributeMask), out var optionAttributeMask))
        {
            PluginLog.Warning($"Failed to build [{nameof(OptionImcAttributeMask)}], required attribute [{nameof(OptionImcAttributeMask.AttributeMask)}] is missing");
        }

        if (!OptionBuilder.TryBuild(jsonElement, out var option))
        {
            PluginLog.Debug($"Failed to build base [{nameof(Option)}] for [{nameof(OptionImcAttributeMask)}]");
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
