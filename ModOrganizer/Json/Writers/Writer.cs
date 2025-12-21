using Dalamud.Plugin.Services;
using System.Collections.Generic;
using System.Text.Json;

namespace ModOrganizer.Json.Writers;

public abstract class Writer<T>(IPluginLog pluginLog) : IWriter<T>
{
    protected IPluginLog PluginLog { get; init; } = pluginLog;

    public abstract bool TryWrite(Utf8JsonWriter jsonWriter, T instance);

    public bool TryWriteMany(Utf8JsonWriter jsonWriter, IEnumerable<T> instances)
    {
        using var _ = jsonWriter.WriteArray();
        foreach (var instance in instances)
        {
            if (!TryWrite(jsonWriter, instance))
            {
                PluginLog.Debug($"Failed to write [{typeof(T).Name}]");
                return false;
            }
        }
        return true;
    }
}
