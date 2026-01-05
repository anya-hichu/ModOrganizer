using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Imcs;

public class MetaImcEntryReader(IAssert assert, IPluginLog pluginLog) : Reader<MetaImcEntry>(assert, pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaImcEntry? instance)
    {
        instance = null;

        if (!Assert.IsValue(element, JsonValueKind.Object)) return false;

        if (!Assert.IsU8Value(element, nameof(MetaImcEntry.MaterialId), out var materialId)) return false;
        if (!Assert.IsU8Value(element, nameof(MetaImcEntry.DecalId), out var decalId)) return false;
        if (!Assert.IsU8Value(element, nameof(MetaImcEntry.VfxId), out var vfxId)) return false;
        if (!Assert.IsU8Value(element, nameof(MetaImcEntry.MaterialAnimationId), out var materialAnimationId)) return false;
        if (!Assert.IsPropertyPresent(element, nameof(MetaImcEntry.AttributeMask), out var attributeMaskIdProperty)) return false;
        if (!Assert.IsPropertyPresent(element, nameof(MetaImcEntry.SoundId), out var soundIdProperty)) return false;

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
