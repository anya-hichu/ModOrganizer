using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers.Asserts;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers;

public abstract class GenericReader<T>(IAssert assert, IPluginLog pluginLog) : Reader<T>(assert, pluginLog) where T : class
{
    protected abstract bool TryGetReader(JsonElement element, [NotNullWhen(true)] out IReader<T>? reader);

    public override bool TryRead(JsonElement element, [NotNullWhen(true)] out T? instance)
    {
        instance = null;

        if (!TryGetReader(element, out var reader)) return false;

        if (!reader.TryRead(element, out instance))
        {
            PluginLog.Debug($"Failed to read [{typeof(T).Name}] using reader [{reader.GetType().Name}]: {element}");
            return false;
        }

        return true;
    }
}
