using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json;

public abstract class TypeFactory<T>(IPluginLog pluginLog) : Factory<T>(pluginLog) where T : class
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
            PluginLog.Warning($"Failed to get [{typeof(T).Name}] builder for type [{type}] (registered types: [{string.Join(", ", Readers.Keys)}]):\n\t{jsonElement}");
            return false;
        }

        return true;
    }
}
