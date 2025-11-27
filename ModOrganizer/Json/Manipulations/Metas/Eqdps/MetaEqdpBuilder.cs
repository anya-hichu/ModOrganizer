using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Eqdps;

public class MetaEqdpBuilder(IPluginLog pluginLog) : Builder<MetaEqdp>(pluginLog)
{
    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out MetaEqdp? instance)
    {
        instance = null;

        if (!Assert.IsObject(jsonElement)) return false;

        if (!Assert.IsPropertyPresent(jsonElement, nameof(MetaEqdp.Entry), out var entryProperty)) return false;
        if (!Assert.IsPropertyValuePresent(jsonElement, nameof(MetaEqdp.Gender), out var gender)) return false;
        if (!Assert.IsPropertyValuePresent(jsonElement, nameof(MetaEqdp.Race), out var race)) return false;
        if (!Assert.IsU16PropertyValue(jsonElement, nameof(MetaEqdp.SetId), out var setId)) return false;
        if (!Assert.IsPropertyValuePresent(jsonElement, nameof(MetaEqdp.Slot), out var slot)) return false;

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
