using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Atrs;

public class MetaAtrReader(IPluginLog pluginLog) : Reader<MetaAtr>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaAtr? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetRequiredNotEmptyPropertyValue(nameof(MetaAtr.Attribute), out var attribute, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(MetaAtr.Entry), out bool? entry, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(MetaAtr.Slot), out string? slot, PluginLog)) return false;
        if (!element.TryGetOptionalU16PropertyValue(nameof(MetaAtr.Id), out var id, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(MetaAtr.GenderRaceCondition), out ushort? genderRaceCondition, PluginLog)) return false;

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
