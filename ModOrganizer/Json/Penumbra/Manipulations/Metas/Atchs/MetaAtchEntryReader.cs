using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Atchs;

public class MetaAtchEntryReader(IPluginLog pluginLog) : Reader<MetaAtchEntry>(pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaAtchEntry? instance)
    {
        instance = null;

        if (!IsValue(element, JsonValueKind.Object)) return false;

        if (!IsValuePresent(element, nameof(MetaAtchEntry.Bone), out var bone)) return false;

        if (!IsPropertyPresent(element, nameof(MetaAtchEntry.Scale), out var scaleProperty)) return false;
        if (!IsPropertyPresent(element, nameof(MetaAtchEntry.OffsetX), out var offsetXProperty)) return false;
        if (!IsPropertyPresent(element, nameof(MetaAtchEntry.OffsetY), out var offsetYProperty)) return false;
        if (!IsPropertyPresent(element, nameof(MetaAtchEntry.OffsetZ), out var offsetZProperty)) return false;
        if (!IsPropertyPresent(element, nameof(MetaAtchEntry.RotationX), out var rotationXProperty)) return false;
        if (!IsPropertyPresent(element, nameof(MetaAtchEntry.RotationY), out var rotationYProperty)) return false;
        if (!IsPropertyPresent(element, nameof(MetaAtchEntry.RotationZ), out var rotationZProperty)) return false;

        instance = new()
        { 
            Bone = bone,
            Scale = scaleProperty.GetSingle(),
            OffsetX = offsetXProperty.GetSingle(),
            OffsetY = offsetYProperty.GetSingle(),
            OffsetZ = offsetZProperty.GetSingle(),
            RotationX = rotationXProperty.GetSingle(),
            RotationY = rotationYProperty.GetSingle(),
            RotationZ = rotationZProperty.GetSingle(),
        };

        return true;
    }
}
