using Dalamud.Plugin.Services;
using ModOrganizer.Json.Penumbra.Options.Imcs.AttributeMasks;
using ModOrganizer.Json.Penumbra.Options.Imcs.IsDisableSubMods;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Options.Imcs.Generics;

public class OptionImcGenericReader(IOptionImcAttributeMaskReader optionImcAttributeMaskReader, IOptionImcIsDisableSubModReader optionImcIsDisabledSubModReader, IPluginLog pluginLog) : GenericReader<OptionImc>(pluginLog), IOptionImcGenericReader
{
    protected override bool TryGetReader(JsonElement element, [NotNullWhen(true)] out IReader<OptionImc>? reader)
    {
        reader = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        var attributeMaskName = nameof(OptionImcAttributeMask.AttributeMask);
        var isDisableSubModName = nameof(OptionImcIsDisableSubMod.IsDisableSubMod);

        switch (element.HasProperty(attributeMaskName, PluginLog), element.HasProperty(isDisableSubModName, PluginLog))
        {
            case (true, true):
                PluginLog.Warning($"Failed to determine reader for [{nameof(OptionImc)}], both properties [{attributeMaskName}] and [{isDisableSubModName}] are present");
                return false;

            case (true, false):
                reader = optionImcAttributeMaskReader;
                return true;

            case (false, true):
                reader = optionImcIsDisabledSubModReader;
                return true;

            case (false, false):
                PluginLog.Warning($"Failed to determine reader for [{nameof(OptionImc)}], both properties [{attributeMaskName}] and [{isDisableSubModName}] are missing");
                break;
        }

        return false;
    }
}
