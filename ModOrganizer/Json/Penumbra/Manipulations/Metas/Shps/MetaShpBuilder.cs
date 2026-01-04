using Dalamud.Plugin.Services;
using ModOrganizer.Json.Asserts;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Atrs;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Shps;

public class MetaShpReader(IAssert assert, IPluginLog pluginLog) : Reader<MetaShp>(assert, pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaShp? instance)
    {
        instance = null;

        if (!Assert.IsValue(element, JsonValueKind.Object)) return false;

        if (!Assert.IsValuePresent(element, nameof(MetaShp.Shape), out var shape)) return false;
        Assert.IsU16Value(element, nameof(MetaShp.Id), out var id, false);

        var entry = element.TryGetProperty(nameof(MetaShp.Entry), out var entryProperty) && entryProperty.GetBoolean();
        var slot = element.TryGetProperty(nameof(MetaShp.Slot), out var slotProperty) ? slotProperty.GetString() : null;
        var connectorCondition = element.TryGetProperty(nameof(MetaShp.Slot), out var connectorConditionProperty) ? connectorConditionProperty.GetString() : null;
        ushort? genderRaceCondition = element.TryGetProperty(nameof(MetaAtr.GenderRaceCondition), out var genderRaceConditionProperty) ?
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
