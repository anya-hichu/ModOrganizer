using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Atrs;

public class MetaAtrReader(IAssert assert, IPluginLog pluginLog) : Reader<MetaAtr>(assert, pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaAtr? instance)
    {
        instance = null;

        if (!Assert.IsValue(element, JsonValueKind.Object)) return false;

        if (!Assert.IsValuePresent(element, nameof(MetaAtr.Attribute), out var attribute)) return false;

        var entry = element.TryGetProperty(nameof(MetaAtr.Entry), out var entryProperty) && entryProperty.GetBoolean();
        var slot = element.TryGetProperty(nameof(MetaAtr.Slot), out var slotProperty) ? slotProperty.GetString() : null;

        Assert.IsU16Value(element, nameof(MetaAtr.Id), out var id, false);
        ushort? genderRaceCondition = element.TryGetProperty(nameof(MetaAtr.GenderRaceCondition), out var genderRaceConditionProperty) ? 
            genderRaceConditionProperty.GetUInt16() : null;

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
