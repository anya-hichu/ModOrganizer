using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Eqdps;

public class MetaEqdpBuilder(IPluginLog pluginLog) : Builder<MetaEqdp>(pluginLog)
{
    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out MetaEqdp? instance)
    {
        instance = null;

        if (!AssertObject(jsonElement)) return false;

        if (!AssertPropertyPresent(jsonElement, nameof(MetaEqdp.Entry), out var entryProperty)) return false;
        if (!AssertPropertyValuePresent(jsonElement, nameof(MetaEqdp.Gender), out var gender)) return false;
        if (!AssertPropertyValuePresent(jsonElement, nameof(MetaEqdp.Race), out var race)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaEqdp.SetId), out var setIdProperty)) return false;
        if (!AssertPropertyValuePresent(jsonElement, nameof(MetaEqdp.Slot), out var slot)) return false;

        // normalize as number => add inside Builder as generic
        if (!(setIdProperty.TryGetUInt16(out var setId) || ushort.TryParse(setIdProperty.GetString(), out setId)))
        {
            PluginLog.Warning($"Failed to build [{nameof(MetaEqdp)}], required attribute [{nameof(MetaEqdp.SetId)}] could not be parsed:\n{jsonElement}");
            return false;
        }

        // TODO REVIEW https://github.com/search?q=repo%3Axivdev%2FPenumbra%20%23U8&type=code and https://github.com/search?q=repo%3Axivdev%2FPenumbra%20%23U16&type=code

        instance = new()
        {
            Entry = entryProperty.GetUInt16(),
            Gender = gender,
            Race = race,
            SetId = setId,
            Slot = slot
        };

        return true;
    }
}
