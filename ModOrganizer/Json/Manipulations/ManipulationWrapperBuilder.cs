using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Manipulations;

public abstract class ManipulationWrapperBuilder<T>(IPluginLog pluginLog, string type) : Builder<ManipulationWrapper>(pluginLog) where T : Manipulation
{
    private string Type { get; init; } = type;

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out ManipulationWrapper? instance)
    {
        instance = default;

        if (!AssertIsObject(jsonElement)) return false;

        if (!AssertHasProperty(jsonElement, nameof(ManipulationWrapper.Manipulation), out var manipulationProperty)) return false;

        if (!TryBuildWrapped(manipulationProperty, out var wrapped))
        {
            PluginLog.Debug($"Failed to build wrapped [{nameof(T)}] for [{nameof(ManipulationWrapper)}]");
            return false;
        }

        instance = new()
        {
            Type = Type,
            Manipulation = wrapped
        };

        return true;
    }

    protected abstract bool TryBuildWrapped(JsonElement jsonElement, [NotNullWhen(true)] out T? instance);
}
