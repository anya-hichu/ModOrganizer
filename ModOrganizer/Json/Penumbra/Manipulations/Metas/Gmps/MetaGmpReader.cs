using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Gmps;

public class MetaGmpReader(IReader<MetaGmpEntry> metaGmpEntryReader, IPluginLog pluginLog) : Reader<MetaGmp>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaGmp? instance)
    {
        instance = null;

        if (!IsValue(element, JsonValueKind.Object)) return false;
        if (!IsPropertyPresent(element, nameof(MetaGmp.Entry), out var entryProperty)) return false;

        if (!metaGmpEntryReader.TryRead(entryProperty, out var entry))
        {
            PluginLog.Debug($"Failed to read [{nameof(MetaGmpEntry)}] for [{nameof(MetaGmp)}]: {entryProperty}");
            return false;
        }

        if (!IsU16Value(element, nameof(MetaGmp.SetId), out var setId)) return false;

        instance = new()
        { 
            Entry = entry,
            SetId = setId
        };

        return true;
    }
}
