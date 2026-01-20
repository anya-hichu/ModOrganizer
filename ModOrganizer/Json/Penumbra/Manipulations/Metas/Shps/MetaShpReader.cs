using Dalamud.Plugin.Services;
using ModOrganizer.Json.Penumbra.Manipulations.Metas.Atrs;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Shps;

public class MetaShpReader(IPluginLog pluginLog) : Reader<MetaShp>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaShp? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetRequiredNotEmptyPropertyValue(nameof(MetaShp.Shape), out var shape, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyU16Value(nameof(MetaShp.Id), out var id, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(MetaShp.Entry), out bool? entry, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(MetaShp.Slot), out string? slot, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(MetaShp.Slot), out string? connectorCondition, PluginLog)) return false;
        if (!element.TryGetOptionalPropertyValue(nameof(MetaShp.GenderRaceCondition), out ushort? genderRaceCondition, PluginLog)) return false;

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
