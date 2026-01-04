using Dalamud.Plugin.Services;
using ModOrganizer.Json.Asserts;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations;

public abstract class ManipulationWrapperReader<T>(IAssert assert, IPluginLog pluginLog, string type) : Reader<ManipulationWrapper>(assert, pluginLog) where T : Manipulation
{
    private string Type { get; init; } = type;

    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out ManipulationWrapper? instance)
    {
        instance = null;

        if (!Assert.IsValue(element, JsonValueKind.Object)) return false;

        if (!Assert.IsPropertyPresent(element, nameof(ManipulationWrapper.Manipulation), out var manipulationProperty)) return false;

        if (!TryReadWrapped(manipulationProperty, out var wrapped))
        {
            PluginLog.Debug($"Failed to read wrapped [{typeof(T).Name}] for [{nameof(ManipulationWrapper)}]: {manipulationProperty}");
            return false;
        }

        instance = new()
        {
            Type = Type,
            Manipulation = wrapped
        };

        return true;
    }

    protected abstract bool TryReadWrapped(JsonElement element, [NotNullWhen(true)] out T? instance);
}
