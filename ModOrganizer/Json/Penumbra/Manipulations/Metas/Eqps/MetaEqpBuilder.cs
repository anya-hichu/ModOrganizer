using Dalamud.Plugin.Services;
using ModOrganizer.Json.Asserts;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Eqps;

public class MetaEqpReader(IAssert assert, IPluginLog pluginLog) : Reader<MetaEqp>(assert, pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaEqp? instance)
    {
        instance = null;

        if (!Assert.IsValue(element, JsonValueKind.Object)) return false;

        if (!Assert.IsPropertyPresent(element, nameof(MetaEqp.Entry), out var entryProperty)) return false;
        if (!Assert.IsU16Value(element, nameof(MetaEqp.SetId), out var setId)) return false;
        if (!Assert.IsValuePresent(element, nameof(MetaEqp.Slot), out var slot)) return false;

        instance = new()
        {
            Entry = entryProperty.GetUInt64(),
            SetId = setId,
            Slot = slot
        };

        return true;
    }
}
