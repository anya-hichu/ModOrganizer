using Dalamud.Plugin.Services;
using ModOrganizer.Json.Asserts;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Ests;

public class MetaEstReader(IAssert assert, IPluginLog pluginLog) : Reader<MetaEst>(assert, pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaEst? instance)
    {
        instance = null;

        if (!Assert.IsValue(element, JsonValueKind.Object)) return false;

        if (!Assert.IsU16Value(element, nameof(MetaEst.Entry), out var entry)) return false;
        if (!Assert.IsValuePresent(element, nameof(MetaEst.Gender), out var gender)) return false;
        if (!Assert.IsValuePresent(element, nameof(MetaEst.Race), out var race)) return false;
        if (!Assert.IsU16Value(element, nameof(MetaEst.SetId), out var setIdProperty)) return false;
        if (!Assert.IsValuePresent(element, nameof(MetaEst.Slot), out var slot)) return false;

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
