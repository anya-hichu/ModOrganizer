using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Asserts;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Atchs;

public class MetaAtchEntryReader(IAssert assert, IPluginLog pluginLog) : Reader<MetaAtchEntry>(assert, pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaAtchEntry? instance)
    {
        instance = null;

        if (!Assert.IsValue(element, JsonValueKind.Object)) return false;

        if (!Assert.IsValuePresent(element, nameof(MetaAtchEntry.Bone), out var bone)) return false;

        if (!Assert.IsPropertyPresent(element, nameof(MetaAtchEntry.Scale), out var scaleProperty)) return false;
        if (!Assert.IsPropertyPresent(element, nameof(MetaAtchEntry.OffsetX), out var offsetXProperty)) return false;
        if (!Assert.IsPropertyPresent(element, nameof(MetaAtchEntry.OffsetY), out var offsetYProperty)) return false;
        if (!Assert.IsPropertyPresent(element, nameof(MetaAtchEntry.OffsetZ), out var offsetZProperty)) return false;
        if (!Assert.IsPropertyPresent(element, nameof(MetaAtchEntry.RotationX), out var rotationXProperty)) return false;
        if (!Assert.IsPropertyPresent(element, nameof(MetaAtchEntry.RotationY), out var rotationYProperty)) return false;
        if (!Assert.IsPropertyPresent(element, nameof(MetaAtchEntry.RotationZ), out var rotationZProperty)) return false;

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
