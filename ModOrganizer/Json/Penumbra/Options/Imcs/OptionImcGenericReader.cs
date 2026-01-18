using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Options.Imcs;

public class OptionImcGenericReader(IOptionImcAttributeMaskReader optionImcAttributeMaskReader, IOptionImcIsDisableSubModReader optionImcIsDisabledSubModReader, IPluginLog pluginLog) : GenericReader<OptionImc>(pluginLog), IOptionImcGenericReader
{
    protected override bool TryGetReader(JsonElement element, [NotNullWhen(true)] out IReader<OptionImc>? builder)
    {
        builder = null;

        var hasAttributeMaskProperty = element.TryGetProperty(nameof(OptionImcAttributeMask.AttributeMask), out var _);
        var hasIsDisableSubModProperty = element.TryGetProperty(nameof(OptionImcIsDisableSubMod.IsDisableSubMod), out var _);

        if (hasAttributeMaskProperty && hasIsDisableSubModProperty)
        {
            PluginLog.Warning($"Failed to determine builder for [{nameof(OptionImc)}], both attributes [{nameof(OptionImcAttributeMask.AttributeMask)}] and [{nameof(OptionImcIsDisableSubMod.IsDisableSubMod)}] are present");
            return false;
        }

        if (hasAttributeMaskProperty)
        {
            builder = optionImcAttributeMaskReader;
            return true;
        }

        if (hasIsDisableSubModProperty)
        {
            builder = optionImcIsDisabledSubModReader;
            return true;
        }

        PluginLog.Warning($"Failed to determine builder for [{nameof(OptionImc)}], both attributes [{nameof(OptionImcAttributeMask.AttributeMask)}] and [{nameof(OptionImcIsDisableSubMod.IsDisableSubMod)}] are missing");
        return false;
    }
}
