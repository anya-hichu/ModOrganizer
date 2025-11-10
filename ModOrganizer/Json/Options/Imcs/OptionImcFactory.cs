using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Options.Imcs;

public class OptionImcFactory(IPluginLog pluginLog) : Factory<OptionImc>(pluginLog)
{
    private OptionImcAttributeMaskBuilder OptionImcAttributeMaskBuilder { get; init; } = new(pluginLog);
    private OptionImcIsDisableSubModBuilder OptionImcIsDisableSubModBuilder { get; init; } = new(pluginLog);

    protected override bool TryGetBuilder(JsonElement jsonElement, [NotNullWhen(true)] out Builder<OptionImc>? builder)
    {
        builder = default;

        var hasAttributeMaskProperty = jsonElement.TryGetProperty(nameof(OptionImcAttributeMask.AttributeMask), out var _);
        var hasIsDisableSubModProperty = jsonElement.TryGetProperty(nameof(OptionImcIsDisableSubMod.IsDisableSubMod), out var _);

        if (hasAttributeMaskProperty && hasIsDisableSubModProperty)
        {
            PluginLog.Warning($"Failed to determine builder for [{nameof(OptionImc)}], both attributes [{nameof(OptionImcAttributeMask.AttributeMask)}] and [{nameof(OptionImcIsDisableSubMod.IsDisableSubMod)}] are present");
            return false;
        }

        if (hasAttributeMaskProperty)
        {
            builder = OptionImcAttributeMaskBuilder;
            return true;
        }

        if (hasIsDisableSubModProperty)
        {
            builder = OptionImcIsDisableSubModBuilder;
            return true;
        }

        PluginLog.Warning($"Failed to determine builder for [{nameof(OptionImc)}], both attributes [{nameof(OptionImcAttributeMask.AttributeMask)}] and [{nameof(OptionImcIsDisableSubMod.IsDisableSubMod)}] are missing");
        return false;
    }
}
