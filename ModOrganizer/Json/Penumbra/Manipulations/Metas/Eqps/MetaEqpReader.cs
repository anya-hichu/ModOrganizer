using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Eqps;

public class MetaEqpReader(IPluginLog pluginLog) : Reader<MetaEqp>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaEqp? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetRequiredPropertyValue(nameof(MetaEqp.Entry), out ulong entry, PluginLog)) return false;
        if (!element.TryGetRequiredU16PropertyValue(nameof(MetaEqp.SetId), out var setId, PluginLog)) return false;
        if (!element.TryGetRequiredNotEmptyPropertyValue(nameof(MetaEqp.Slot), out var slot, PluginLog)) return false;

        instance = new()
        {
            Entry = entry,
            SetId = setId,
            Slot = slot
        };

        return true;
    }
}
