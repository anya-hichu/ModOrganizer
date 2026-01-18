using Dalamud.Plugin.Services;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers;

public abstract class TypeGenericReader<T>(IPluginLog pluginLog) : GenericReader<T>(pluginLog) where T : class
{
    private static readonly string TYPE_PROPERTY_NAME = "Type";

    protected Dictionary<string, IReader<T>> Readers { get; init; } = [];

    protected override bool TryGetReader(JsonElement element, [NotNullWhen(true)] out IReader<T>? reader)
    {
        reader = null;

        if (!IsValue(element, JsonValueKind.Object)) return false;

        if (!IsValuePresent(element, TYPE_PROPERTY_NAME, out var type)) return false;

        if (!Readers.TryGetValue(type, out reader))
        {
            PluginLog.Warning($"Failed to get [{typeof(T).Name}] reader for type [{type}] (registered types: {string.Join(", ", Readers.Keys)}): {element}");
            return false;
        }

        return true;
    }


}
