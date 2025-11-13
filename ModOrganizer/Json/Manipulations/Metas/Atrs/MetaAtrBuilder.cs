using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Atrs;

public class MetaAtrBuilder(IPluginLog pluginLog) : Builder<MetaAtr>(pluginLog)
{
    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out MetaAtr? instance)
    {
        instance = null;

        if (!AssertObject(jsonElement)) return false;

        if (!AssertPropertyValuePresent(jsonElement, nameof(MetaAtr.Attribute), out var attribute)) return false;

        var entry = jsonElement.TryGetProperty(nameof(MetaAtr.Entry), out var entryProperty) && entryProperty.GetBoolean();
        var slot = jsonElement.TryGetProperty(nameof(MetaAtr.Slot), out var slotProperty) ? slotProperty.GetString() : null;

        AssertU16PropertyValue(jsonElement, nameof(MetaAtr.Id), out var id, false);
        uint? genderRaceCondition = jsonElement.TryGetProperty(nameof(MetaAtr.GenderRaceCondition), out var genderRaceConditionProperty) ? 
            genderRaceConditionProperty.GetUInt32() : null;

        instance = new()
        { 
            Entry = entry,
            Slot = slot,
            Id = id,
            Attribute = attribute,
            GenderRaceCondition = genderRaceCondition
        };

        return true;
    }
}
