using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Penumbra.Manipulations.Metas.Ests;

public class MetaEstReader(IPluginLog pluginLog) : Reader<MetaEst>(pluginLog)
{
    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out MetaEst? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        if (!Assert.IsU16Value(jsonElement, nameof(MetaEst.Entry), out var entry)) return false;
        if (!Assert.IsValuePresent(jsonElement, nameof(MetaEst.Gender), out var gender)) return false;
        if (!Assert.IsValuePresent(jsonElement, nameof(MetaEst.Race), out var race)) return false;
        if (!Assert.IsU16Value(jsonElement, nameof(MetaEst.SetId), out var setIdProperty)) return false;
        if (!Assert.IsValuePresent(jsonElement, nameof(MetaEst.Slot), out var slot)) return false;

        instance = new()
        {
            Entry = entry,
            Gender = gender,
            Race = race,
            SetId = setIdProperty,
            Slot = slot
        };

        return true;
    }
}
