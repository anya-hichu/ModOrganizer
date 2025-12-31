using Dalamud.Plugin.Services;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers;

public abstract class TypeReaderFactory<T>(IPluginLog pluginLog) : ReaderFactory<T>(pluginLog) where T : class
{
    private static readonly string TYPE_PROPERTY_NAME = "Type";

    protected Dictionary<string, IReader<T>> Readers { get; init; } = [];

    protected override bool TryGetReader(JsonElement jsonElement, [NotNullWhen(true)] out IReader<T>? reader)
    {
        reader = null;

        if (!Assert.IsValue(jsonElement, JsonValueKind.Object)) return false;

        if (!Assert.IsValuePresent(jsonElement, TYPE_PROPERTY_NAME, out var type)) return false;

        if (!Readers.TryGetValue(type, out reader))
        {
            PluginLog.Warning($"Failed to get [{typeof(T).Name}] builder for type [{type}] (registered types: [{string.Join(", ", Readers.Keys)}]): {jsonElement}");
            return false;
        }

        return true;
    }
}
