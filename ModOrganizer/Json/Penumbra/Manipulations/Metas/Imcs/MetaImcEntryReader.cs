using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Penumbra.Manipulations.Metas.Imcs;

public class MetaImcEntryReader(IPluginLog pluginLog) : Reader<MetaImcEntry>(pluginLog)
{
    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out MetaImcEntry? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        if (!Assert.IsU8Value(jsonElement, nameof(MetaImcEntry.MaterialId), out var materialId)) return false;
        if (!Assert.IsU8Value(jsonElement, nameof(MetaImcEntry.DecalId), out var decalId)) return false;
        if (!Assert.IsU8Value(jsonElement, nameof(MetaImcEntry.VfxId), out var vfxId)) return false;
        if (!Assert.IsU8Value(jsonElement, nameof(MetaImcEntry.MaterialAnimationId), out var materialAnimationId)) return false;
        if (!Assert.IsPropertyPresent(jsonElement, nameof(MetaImcEntry.AttributeMask), out var attributeMaskIdProperty)) return false;
        if (!Assert.IsPropertyPresent(jsonElement, nameof(MetaImcEntry.SoundId), out var soundIdProperty)) return false;

        instance = new()
        {
            MaterialId = materialId,
            DecalId = decalId,
            VfxId = vfxId,
            MaterialAnimationId = materialAnimationId,
            AttributeMask = attributeMaskIdProperty.GetUInt16(),
            SoundId = soundIdProperty.GetByte()
        };

        return true;
    }
}
