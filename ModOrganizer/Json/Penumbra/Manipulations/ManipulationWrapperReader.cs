using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using ModOrganizer.Json.Readers.Elements;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Penumbra.Manipulations;

public abstract class ManipulationWrapperReader<T>(IPluginLog pluginLog, string type) : Reader<ManipulationWrapper>(pluginLog) where T : Manipulation
{
    private string Type { get; init; } = type;

    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out ManipulationWrapper? instance)
    {
        instance = null;

        if (!element.Is(JsonValueKind.Object, PluginLog)) return false;

        if (!element.TryGetRequiredProperty(nameof(ManipulationWrapper.Manipulation), out var manipulationProperty, PluginLog)) return false;

        if (!TryReadWrapped(manipulationProperty, out var wrapped))
        {
            PluginLog.Debug($"Failed to read wrapped [{typeof(T).Name}] for [{nameof(ManipulationWrapper)}]: {element}");
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
