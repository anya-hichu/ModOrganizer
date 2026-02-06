using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs.Entries;

public class MetaImcEntryReader(IPluginLog pluginLog) : Reader<MetaImcEntry>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaImcEntry? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetRequiredU8PropertyValue(nameof(MetaImcEntry.MaterialId), out var materialId, PluginLog)) return false;
        if (!element.TryGetRequiredU8PropertyValue(nameof(MetaImcEntry.DecalId), out var decalId, PluginLog)) return false;
        if (!element.TryGetRequiredU8PropertyValue(nameof(MetaImcEntry.VfxId), out var vfxId, PluginLog)) return false;
        if (!element.TryGetRequiredU8PropertyValue(nameof(MetaImcEntry.MaterialAnimationId), out var materialAnimationId, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(MetaImcEntry.AttributeMask), out ushort attributeMask, PluginLog)) return false;
        if (!element.TryGetRequiredPropertyValue(nameof(MetaImcEntry.SoundId), out byte soundId, PluginLog)) return false;

        instance = new()
        {
            MaterialId = materialId,
            DecalId = decalId,
            VfxId = vfxId,
            MaterialAnimationId = materialAnimationId,
            AttributeMask = attributeMask,
            SoundId = soundId
        };

        return true;
    }
}
