using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json;

public abstract class Builder<T>(IPluginLog pluginLog) where T : class
{
    public IPluginLog PluginLog { get; init; } = pluginLog;
    protected Assert Assert { get; init; } = new(pluginLog);

    public abstract bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out T? instance);
}
