using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Options.Imcs;

public class OptionImcGenericReader(IOptionImcAttributeMaskReader optionImcAttributeMaskReader, IOptionImcIsDisableSubModReader optionImcIsDisabledSubModReader, IPluginLog pluginLog) : GenericReader<OptionImc>(pluginLog), IOptionImcGenericReader
{
    protected override bool TryGetReader(JsonElement element, [NotNullWhen(true)] out IReader<OptionImc>? builder)
    {
        builder = null;

        var attributeMaskName = nameof(OptionImcAttributeMask.AttributeMask);
        var isDisableSubModName = nameof(OptionImcIsDisableSubMod.IsDisableSubMod);

        switch (element.HasProperty(attributeMaskName), element.HasProperty(isDisableSubModName))
        {
            case (true, true):
                PluginLog.Warning($"Failed to determine builder for [{nameof(OptionImc)}], both attributes [{attributeMaskName}] and [{isDisableSubModName}] are present");
                return false;

            case (true, false):
                builder = optionImcAttributeMaskReader;
                return true;

            case (false, true):
                builder = optionImcIsDisabledSubModReader;
                return true;

            case (false, false):
                PluginLog.Warning($"Failed to determine builder for [{nameof(OptionImc)}], both attributes [{attributeMaskName}] and [{isDisableSubModName}] are missing");
                break;
        }

        return false;
    }
}
