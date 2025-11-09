using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Options.Imcs;

public class OptionImcIsDisableSubModBuilder(IPluginLog pluginLog) : Builder<OptionImc>(pluginLog)
{
    private OptionBuilder OptionBuilder { get; init; } = new(pluginLog);

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out OptionImc? instance)
    {
        instance = default;

        if (!jsonElement.TryGetProperty(nameof(OptionImcIsDisableSubMod.IsDisableSubMod), out var optionIsDisableSubMod))
        {
            PluginLog.Warning($"Failed to build [{nameof(OptionImcAttributeMask)}], required attribute [{nameof(OptionImcIsDisableSubMod.IsDisableSubMod)}] is missing");
        }

        if (!OptionBuilder.TryBuild(jsonElement, out var option))
        {
            PluginLog.Debug($"Failed to build base [{nameof(Option)}] for [{nameof(OptionImcIsDisableSubMod)}]");
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
