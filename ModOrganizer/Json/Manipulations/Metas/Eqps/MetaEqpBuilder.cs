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

        if (!AssertObject(jsonElement)) return false;

        if (!AssertPropertyPresent(jsonElement, nameof(MetaEqp.Entry), out var entryProperty)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaEqp.SetId), out var setIdProperty)) return false;
        if (!AssertPropertyValuePresent(jsonElement, nameof(MetaEqp.Slot), out var slot)) return false;

        // normalize as number
        if (!(setIdProperty.TryGetUInt16(out var setId) || ushort.TryParse(setIdProperty.GetString(), out setId)))
        {
            PluginLog.Warning($"Failed to build [{nameof(MetaEqdp)}], required attribute [{nameof(MetaEqdp.SetId)}] could not be parsed:\n{jsonElement}");
            return false;
        }

        instance = new()
        {
            Entry = entryProperty.GetUInt64(),
            SetId = setId,
            Slot = slot
        };

        return true;
    }
}
