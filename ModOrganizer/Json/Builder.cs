using Dalamud.Plugin.Services;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json;

public abstract class Builder<T>(IPluginLog pluginLog) where T : class
{
    public IPluginLog PluginLog { get; init; } = pluginLog;
    protected Assert Assert { get; init; } = new(pluginLog);

    public abstract bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out T? instance);

    public bool TryBuildMany(JsonElement jsonElement, [NotNullWhen(true)] out T[]? instances)
    {
        instances = null;
        if (!Assert.IsArray(jsonElement)) return false;

        var list = new List<T>();
        foreach (var item in jsonElement.EnumerateArray())
        {
            if (!TryBuild(item, out var instance))
            {
                PluginLog.Debug($"Failed to build [{typeof(T).Name}]:\n\t{item}");
                return false;
            }
            list.Add(instance);
        }

        instances = [.. list];
        return true;
    }
}
