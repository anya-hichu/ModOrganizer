using Dalamud.Plugin.Services;
using ModOrganizer.Json.Manipulations.Metas.Eqdps;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Eqps;

public class MetaEqpBuilder(IPluginLog pluginLog) : Builder<MetaEqp>(pluginLog)
{
    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out MetaEqp? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        if (!Assert.IsPropertyPresent(jsonElement, nameof(MetaEqp.Entry), out var entryProperty)) return false;
        if (!Assert.IsU16Value(jsonElement, nameof(MetaEqp.SetId), out var setId)) return false;
        if (!Assert.IsValuePresent(jsonElement, nameof(MetaEqp.Slot), out var slot)) return false;

        instance = new()
        {
            Entry = entryProperty.GetUInt64(),
            SetId = setId,
            Slot = slot
        };

        return true;
    }
}
