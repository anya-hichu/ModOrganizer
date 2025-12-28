using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Atrs;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Shps;

public class MetaShpReader(IPluginLog pluginLog) : Reader<MetaShp>(pluginLog)
{
    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out MetaShp? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        if (!Assert.IsValuePresent(jsonElement, nameof(MetaShp.Shape), out var shape)) return false;
        Assert.IsU16Value(jsonElement, nameof(MetaShp.Id), out var id, false);

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
