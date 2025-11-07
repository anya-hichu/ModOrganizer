using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations.Wrapped;

public class MetaImcBuilder(IPluginLog pluginLog) : Builder<ManipulationWrapper>(pluginLog)
{
    public static readonly string TYPE = "Imc";

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out ManipulationWrapper? instance)
    {
        instance = default;

        if (jsonElement.ValueKind != JsonValueKind.Object)
        {
            PluginLog.Warning($"Failed to build [{nameof(ManipulationWrapper)}], expected root object but got [{jsonElement.ValueKind}]");
            return false;
        }

        if (!jsonElement.TryGetProperty(nameof(ManipulationWrapper.Manipulation), out var manipulationProperty))
        {
            PluginLog.Warning($"Failed to build [{nameof(ManipulationWrapper)}], required attribute [{nameof(ManipulationWrapper.Manipulation)}] is missing");
            return false;
        }

        if (!TryBuildUnwrapped(manipulationProperty, out var manipulation))
        {
            PluginLog.Warning($"Failed to build [{nameof(ManipulationWrapper)}], required attribute [{nameof(ManipulationWrapper.Manipulation)}] could not be parsed");
            return false;
        }

        instance = new()
        {
            Type = TYPE,
            Manipulation = manipulation
        };

        return true;
    }

    public bool TryBuildUnwrapped(JsonElement jsonElement, [NotNullWhen(true)] out MetaImc? instance)
    {
        instance = default;

        if (jsonElement.ValueKind != JsonValueKind.Object)
        {
            PluginLog.Warning($"Failed to build [{nameof(Manipulation)}], expected root object but got [{jsonElement.ValueKind}]");
            return false;
        }

        instance = new();

        return true;
    }
}
