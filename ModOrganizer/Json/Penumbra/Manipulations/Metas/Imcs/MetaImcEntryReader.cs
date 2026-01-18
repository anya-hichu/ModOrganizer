using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs;

public class MetaImcEntryReader(IPluginLog pluginLog) : Reader<MetaImcEntry>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaImcEntry? instance)
    {
        instance = null;

        if (!IsValue(element, JsonValueKind.Object)) return false;

        if (!IsU8Value(element, nameof(MetaImcEntry.MaterialId), out var materialId)) return false;
        if (!IsU8Value(element, nameof(MetaImcEntry.DecalId), out var decalId)) return false;
        if (!IsU8Value(element, nameof(MetaImcEntry.VfxId), out var vfxId)) return false;
        if (!IsU8Value(element, nameof(MetaImcEntry.MaterialAnimationId), out var materialAnimationId)) return false;
        if (!IsPropertyPresent(element, nameof(MetaImcEntry.AttributeMask), out var attributeMaskIdProperty)) return false;
        if (!IsPropertyPresent(element, nameof(MetaImcEntry.SoundId), out var soundIdProperty)) return false;

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
