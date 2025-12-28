using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations;

public abstract class ManipulationWrapperReader<T>(IPluginLog pluginLog, string type) : Reader<ManipulationWrapper>(pluginLog) where T : Manipulation
{
    private string Type { get; init; } = type;

    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out ManipulationWrapper? instance)
    {
        instance = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        if (!Assert.IsPropertyPresent(jsonElement, nameof(ManipulationWrapper.Manipulation), out var manipulationProperty)) return false;

        if (!TryReadWrapped(manipulationProperty, out var wrapped))
        {
            PluginLog.Debug($"Failed to read wrapped [{typeof(T).Name}] for [{nameof(ManipulationWrapper)}]:\n\t{manipulationProperty}");
            return false;
        }

        instance = new()
        {
            Type = Type,
            Manipulation = wrapped
        };

        return true;
    }

    protected abstract bool TryReadWrapped(JsonElement jsonElement, [NotNullWhen(true)] out T? instance);
}
