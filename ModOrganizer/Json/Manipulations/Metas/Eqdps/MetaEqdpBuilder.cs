using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Eqdps;

public class MetaEqdpBuilder(IPluginLog pluginLog) : Builder<MetaEqdp>(pluginLog)
{
    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out MetaEqdp? instance)
    {
        instance = null;

        if (!AssertObject(jsonElement)) return false;

        if (!AssertPropertyPresent(jsonElement, nameof(MetaEqdp.Entry), out var entryProperty)) return false;
        if (!AssertPropertyValuePresent(jsonElement, nameof(MetaEqdp.Gender), out var gender)) return false;
        if (!AssertPropertyValuePresent(jsonElement, nameof(MetaEqdp.Race), out var race)) return false;
        if (!AssertU16PropertyValue(jsonElement, nameof(MetaEqdp.SetId), out var setId)) return false;
        if (!AssertPropertyValuePresent(jsonElement, nameof(MetaEqdp.Slot), out var slot)) return false;

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
