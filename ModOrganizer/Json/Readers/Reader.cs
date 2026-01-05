using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers.Asserts;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers;

public abstract class Reader<T>(IAssert assert, IPluginLog pluginLog) : IReader<T> where T : class
{
    protected IAssert Assert { get; init; } = assert;
    public IPluginLog PluginLog { get; init; } = pluginLog;

    public abstract bool TryRead(JsonElement element, [NotNullWhen(true)] out T? instance);

    public bool TryReadMany(JsonElement element, [NotNullWhen(true)] out T[]? instances)
    {
        instances = null;
        if (!Assert.IsValue(element, JsonValueKind.Array)) return false;

        var list = new List<T>();
        foreach (var item in element.EnumerateArray())
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
