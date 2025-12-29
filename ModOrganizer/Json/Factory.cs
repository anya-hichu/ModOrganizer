using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json;

public abstract class Factory<T>(IPluginLog pluginLog) : Reader<T>(pluginLog) where T : class
{
    protected abstract bool TryGetReader(JsonElement jsonElement, [NotNullWhen(true)] out IReader<T>? reader);

    public override bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out T? instance)
    {
        instance = null;

        if (!TryGetReader(jsonElement, out var reader))
        {
            PluginLog.Debug($"Failed to get [{typeof(T).Name}] reader: {jsonElement}");
            return false;
        }

        if (!reader.TryRead(jsonElement, out instance))
        {
            PluginLog.Debug($"Failed to read [{typeof(T).Name}] using reader [{reader.GetType().Name}]: {jsonElement}");
            return false;
        }

        return true;
    }
}
