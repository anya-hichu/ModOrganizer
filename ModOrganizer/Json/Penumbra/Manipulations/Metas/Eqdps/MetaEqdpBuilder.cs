using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Eqdps;

public class MetaEqdpReader(IAssert assert, IPluginLog pluginLog) : Reader<MetaEqdp>(assert, pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaEqdp? instance)
    {
        instance = null;

        if (!Assert.IsValue(element, JsonValueKind.Object)) return false;

        if (!Assert.IsPropertyPresent(element, nameof(MetaEqdp.Entry), out var entryProperty)) return false;
        if (!Assert.IsValuePresent(element, nameof(MetaEqdp.Gender), out var gender)) return false;
        if (!Assert.IsValuePresent(element, nameof(MetaEqdp.Race), out var race)) return false;
        if (!Assert.IsU16Value(element, nameof(MetaEqdp.SetId), out var setId)) return false;
        if (!Assert.IsValuePresent(element, nameof(MetaEqdp.Slot), out var slot)) return false;

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
