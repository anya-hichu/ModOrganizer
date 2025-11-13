using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Ests;

public class MetaEstBuilder(IPluginLog pluginLog) : Builder<MetaEst>(pluginLog)
{
    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out MetaEst? instance)
    {
        instance = null;

        if (!AssertObject(jsonElement)) return false;

        if (!AssertU16PropertyValue(jsonElement, nameof(MetaEst.Entry), out var entry)) return false;
        if (!AssertPropertyValuePresent(jsonElement, nameof(MetaEst.Gender), out var gender)) return false;
        if (!AssertPropertyValuePresent(jsonElement, nameof(MetaEst.Race), out var race)) return false;
        if (!AssertU16PropertyValue(jsonElement, nameof(MetaEst.SetId), out var setIdProperty)) return false;
        if (!AssertPropertyValuePresent(jsonElement, nameof(MetaEst.Slot), out var slot)) return false;

        instance = new()
        {
            Entry = entry,
            Gender = gender,
            Race = race,
            SetId = setIdProperty,
            Slot = slot
        };

        return true;
    }
}
