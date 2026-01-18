using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Eqps;

public class MetaEqpReader(IPluginLog pluginLog) : Reader<MetaEqp>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaEqp? instance)
    {
        instance = null;

        if (!IsValue(element, JsonValueKind.Object)) return false;

        if (!TryGetRequiredProperty(element, nameof(MetaEqp.Entry), out var entryProperty)) return false;
        if (!TryGetRequiredU16Value(element, nameof(MetaEqp.SetId), out var setId)) return false;
        if (!TryGetRequiredValue(element, nameof(MetaEqp.Slot), out var slot)) return false;

        instance = new()
        {
            Entry = entryProperty.GetUInt64(),
            SetId = setId,
            Slot = slot
        };

        return true;
    }
}
