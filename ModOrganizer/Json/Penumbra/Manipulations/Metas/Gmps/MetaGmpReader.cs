using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Gmps;

public class MetaGmpReader(IReader<MetaGmpEntry> metaGmpEntryReader, IPluginLog pluginLog) : Reader<MetaGmp>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaGmp? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetRequiredProperty(nameof(MetaGmp.Entry), out var entryProperty, PluginLog)) return false;

        if (!metaGmpEntryReader.TryRead(entryProperty, out var entry))
        {
            PluginLog.Debug($"Failed to read [{nameof(MetaGmpEntry)}] for [{nameof(MetaGmp)}]: {entryProperty}");
            return false;
        }

        if (!element.TryGetRequiredU16PropertyValue(nameof(MetaGmp.SetId), out var setId, PluginLog)) return false;

        instance = new()
        { 
            Entry = entry,
            SetId = setId
        };

        return true;
    }
}
