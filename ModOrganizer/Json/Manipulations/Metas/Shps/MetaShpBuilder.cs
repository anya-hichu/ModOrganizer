using Dalamud.Plugin.Services;
using ModOrganizer.Json.Manipulations.Metas.Atrs;
using ModOrganizer.Json.Manipulations.Metas.Eqdps;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Shps;

public class MetaShpBuilder(IPluginLog pluginLog) : Builder<MetaShp>(pluginLog)
{
    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out MetaShp? instance)
    {
        instance = null;

        if (!AssertObject(jsonElement)) return false;

        if (!AssertPropertyValuePresent(jsonElement, nameof(MetaShp.Shape), out var shape)) return false;
        AssertU16PropertyValue(jsonElement, nameof(MetaShp.Id), out var id, false);

        var entry = jsonElement.TryGetProperty(nameof(MetaShp.Entry), out var entryProperty) && entryProperty.GetBoolean();
        var slot = jsonElement.TryGetProperty(nameof(MetaShp.Slot), out var slotProperty) ? slotProperty.GetString() : null;
        var connectorCondition = jsonElement.TryGetProperty(nameof(MetaShp.Slot), out var connectorConditionProperty) ? connectorConditionProperty.GetString() : null;
        ushort? genderRaceCondition = jsonElement.TryGetProperty(nameof(MetaAtr.GenderRaceCondition), out var genderRaceConditionProperty) ?
            genderRaceConditionProperty.GetUInt16() : null;

        instance = new()
        {
            Entry = entry,
            Slot = slot,
            Id = id,
            Shape = shape,
            ConnectorCondition = connectorCondition,
            GenderRaceCondition = genderRaceCondition
        };

        return true;
    }
}
