using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json;

public abstract class Builder<T>(IPluginLog pluginLog)
{
    public IPluginLog PluginLog { get; set; } = pluginLog;

    public abstract bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out T? instance);
}
