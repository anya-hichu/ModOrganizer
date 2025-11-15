using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Gmps;

public class MetaGmpBuilder(IPluginLog pluginLog) : Builder<MetaGmp>(pluginLog)
{
    private MetaGmpEntryBuilder MetaGmpEntryBuilder { get; init; } = new(pluginLog);

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out MetaGmp? instance)
    {
        instance = null;

        if (!AssertObject(jsonElement)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaGmp.Entry), out var entryProperty)) return false;

        if (!MetaGmpEntryBuilder.TryBuild(entryProperty, out var entry))
        {
            PluginLog.Debug($"Failed to build [{nameof(MetaGmpEntry)}] for [{nameof(MetaGmp)}]:\n\t{entryProperty}");
            return false;
        }

        if (!AssertU16PropertyValue(jsonElement, nameof(MetaGmp.SetId), out var setId)) return false;

        instance = new()
        { 
            Entry = entry,
            SetId = setId
        };

        return true;
    }
}
