using Dalamud.Plugin.Services;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers;

public abstract class Reader<T>(IPluginLog pluginLog) : IReader<T> where T : class
{
    public IPluginLog PluginLog { get; init; } = pluginLog;
    protected Assert Assert { get; init; } = new(pluginLog);

    public abstract bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out T? instance);

    public bool TryReadMany(JsonElement jsonElement, [NotNullWhen(true)] out T[]? instances)
    {
        instances = null;
        if (!Assert.IsValue(jsonElement, JsonValueKind.Array)) return false;

        var list = new List<T>();
        foreach (var item in jsonElement.EnumerateArray())
        {
            if (!TryRead(item, out var instance))
            {
                PluginLog.Debug($"Failed to read [{typeof(T).Name}]: {item}");
                return false;
            }
            list.Add(instance);
        }

        instances = [.. list];
        return true;
    }
}
