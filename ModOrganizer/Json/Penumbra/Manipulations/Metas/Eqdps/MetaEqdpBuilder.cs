using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Eqdps;

public class MetaEqdpReader(IPluginLog pluginLog) : Reader<MetaEqdp>(pluginLog)
{
    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out MetaEqdp? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        if (!Assert.IsPropertyPresent(jsonElement, nameof(MetaEqdp.Entry), out var entryProperty)) return false;
        if (!Assert.IsValuePresent(jsonElement, nameof(MetaEqdp.Gender), out var gender)) return false;
        if (!Assert.IsValuePresent(jsonElement, nameof(MetaEqdp.Race), out var race)) return false;
        if (!Assert.IsU16Value(jsonElement, nameof(MetaEqdp.SetId), out var setId)) return false;
        if (!Assert.IsValuePresent(jsonElement, nameof(MetaEqdp.Slot), out var slot)) return false;

        instance = new()
        {
            Entry = entryProperty.GetUInt16(),
            Gender = gender,
            Race = race,
            SetId = setId,
            Slot = slot
        };

        return true;
    }
}
