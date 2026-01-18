using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Eqdps;

public class MetaEqdpReader(IPluginLog pluginLog) : Reader<MetaEqdp>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaEqdp? instance)
    {
        instance = null;

        if (!IsValue(element, JsonValueKind.Object)) return false;

        if (!TryGetRequiredProperty(element, nameof(MetaEqdp.Entry), out var entryProperty)) return false;
        if (!TryGetRequiredValue(element, nameof(MetaEqdp.Gender), out var gender)) return false;
        if (!TryGetRequiredValue(element, nameof(MetaEqdp.Race), out var race)) return false;
        if (!TryGetRequiredU16Value(element, nameof(MetaEqdp.SetId), out var setId)) return false;
        if (!TryGetRequiredValue(element, nameof(MetaEqdp.Slot), out var slot)) return false;

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
