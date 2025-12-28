using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Options.Imcs;

public class OptionImcFactory(IPluginLog pluginLog) : Factory<OptionImc>(pluginLog)
{
    private OptionImcAttributeMaskReader OptionImcAttributeMaskReader { get; init; } = new(pluginLog);
    private OptionImcIsDisableSubModReader OptionImcIsDisableSubModReader { get; init; } = new(pluginLog);

    protected override bool TryGetReader(JsonElement jsonElement, [NotNullWhen(true)] out IReader<OptionImc>? builder)
    {
        builder = null;

        var hasAttributeMaskProperty = jsonElement.TryGetProperty(nameof(OptionImcAttributeMask.AttributeMask), out var _);
        var hasIsDisableSubModProperty = jsonElement.TryGetProperty(nameof(OptionImcIsDisableSubMod.IsDisableSubMod), out var _);

        if (hasAttributeMaskProperty && hasIsDisableSubModProperty)
        {
            PluginLog.Warning($"Failed to determine builder for [{nameof(OptionImc)}], both attributes [{nameof(OptionImcAttributeMask.AttributeMask)}] and [{nameof(OptionImcIsDisableSubMod.IsDisableSubMod)}] are present");
            return false;
        }

        if (hasAttributeMaskProperty)
        {
            builder = OptionImcAttributeMaskReader;
            return true;
        }

        if (hasIsDisableSubModProperty)
        {
            builder = OptionImcIsDisableSubModReader;
            return true;
        }

        PluginLog.Warning($"Failed to determine builder for [{nameof(OptionImc)}], both attributes [{nameof(OptionImcAttributeMask.AttributeMask)}] and [{nameof(OptionImcIsDisableSubMod.IsDisableSubMod)}] are missing");
        return false;
    }
}
