using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Eqdps;

public class MetaEqdpReader(IPluginLog pluginLog) : Reader<MetaEqdp>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaEqdp? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetRequiredPropertyValue(nameof(MetaEqdp.Entry), out ushort entry, PluginLog)) return false;
        if (!element.TryGetRequiredNotEmptyPropertyValue(nameof(MetaEqdp.Gender), out var gender, PluginLog)) return false;
        if (!element.TryGetRequiredNotEmptyPropertyValue(nameof(MetaEqdp.Race), out var race, PluginLog)) return false;
        if (!element.TryGetRequiredU16PropertyValue(nameof(MetaEqdp.SetId), out var setId, PluginLog)) return false;
        if (!element.TryGetRequiredNotEmptyPropertyValue(nameof(MetaEqdp.Slot), out var slot, PluginLog)) return false;

        instance = new()
        {
            Entry = entry,
            Gender = gender,
            Race = race,
            SetId = setId,
            Slot = slot
        };

        return true;
    }
}
