using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Penumbra.Manipulations.Metas.Gmps;

public class MetaGmpReader(IPluginLog pluginLog) : Reader<MetaGmp>(pluginLog)
{
    private MetaGmpEntryReader MetaGmpEntryReader { get; init; } = new(pluginLog);

    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out MetaGmp? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;
        if (!Assert.IsPropertyPresent(jsonElement, nameof(MetaGmp.Entry), out var entryProperty)) return false;

        if (!MetaGmpEntryReader.TryRead(entryProperty, out var entry))
        {
            PluginLog.Debug($"Failed to read [{nameof(MetaGmpEntry)}] for [{nameof(MetaGmp)}]:\n\t{entryProperty}");
            return false;
        }

        if (!Assert.IsU16Value(jsonElement, nameof(MetaGmp.SetId), out var setId)) return false;

        instance = new()
        { 
            Entry = entry,
            SetId = setId
        };

        return true;
    }
}
