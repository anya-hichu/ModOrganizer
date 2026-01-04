using Dalamud.Plugin.Services;
using ModOrganizer.Json.Asserts;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations.Metas.Gmps;

public class MetaGmpEntryReader(IAssert assert, IPluginLog pluginLog) : Reader<MetaGmpEntry>(assert, pluginLog)
{
    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out MetaGmpEntry? instance)
    {
        instance = null;

        if (!Assert.IsValue(element, JsonValueKind.Object)) return false;

        if (!Assert.IsPropertyPresent(element, nameof(MetaGmpEntry.Enabled), out var enabledProperty)) return false;
        if (!Assert.IsPropertyPresent(element, nameof(MetaGmpEntry.Animated), out var animatedProperty)) return false;
        if (!Assert.IsPropertyPresent(element, nameof(MetaGmpEntry.RotationA), out var rotationAProperty)) return false;
        if (!Assert.IsPropertyPresent(element, nameof(MetaGmpEntry.RotationB), out var rotationBProperty)) return false;
        if (!Assert.IsPropertyPresent(element, nameof(MetaGmpEntry.RotationC), out var rotationCProperty)) return false;
        if (!Assert.IsPropertyPresent(element, nameof(MetaGmpEntry.UnknownA), out var unknownAProperty)) return false;
        if (!Assert.IsPropertyPresent(element, nameof(MetaGmpEntry.UnknownB), out var unknownBProperty)) return false;

        instance = new()
        {
            Enabled = enabledProperty.GetBoolean(),
            Animated = animatedProperty.GetBoolean(),
            RotationA = rotationAProperty.GetUInt16(),
            RotationB = rotationBProperty.GetUInt16(),
            RotationC = rotationCProperty.GetUInt16(),
            UnknownA = unknownAProperty.GetByte(),
            UnknownB = unknownBProperty.GetByte()
        };

        return true;
    }
}
