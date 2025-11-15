using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Metas.Gmps;

public class MetaGmpEntryBuilder(IPluginLog pluginLog) : Builder<MetaGmpEntry>(pluginLog)
{
    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out MetaGmpEntry? instance)
    {
        instance = null;

        if (!AssertObject(jsonElement)) return false;

        if (!AssertPropertyPresent(jsonElement, nameof(MetaGmpEntry.Enabled), out var enabledProperty)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaGmpEntry.Animated), out var animatedProperty)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaGmpEntry.RotationA), out var rotationAProperty)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaGmpEntry.RotationB), out var rotationBProperty)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaGmpEntry.RotationC), out var rotationCProperty)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaGmpEntry.UnknownA), out var unknownAProperty)) return false;
        if (!AssertPropertyPresent(jsonElement, nameof(MetaGmpEntry.UnknownB), out var unknownBProperty)) return false;

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
