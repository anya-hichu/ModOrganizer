using Dalamud.Plugin.Services;
using Dalamud.Utility;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json;

public abstract class Factory<T>(IPluginLog pluginLog)
{
    private static readonly string TYPE_PROPERTY_NAME = "Type";

    public IPluginLog PluginLog { get; init; } = pluginLog;

    protected Dictionary<string, Builder<T>> Builders { get; init; } = [];

    public bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out T? instance)
    {
        instance = default;

        if (jsonElement.ValueKind != JsonValueKind.Object)
        {
            PluginLog.Warning($"Failed to find [{typeof(T).Name}] builder, expected root object but got [{jsonElement.ValueKind}]");
            return false;
        }

        if (!jsonElement.TryGetProperty(TYPE_PROPERTY_NAME, out var typeProperty))
        {
            PluginLog.Warning($"Failed to find [{typeof(T).Name}] builder, required attribute [{TYPE_PROPERTY_NAME}] is missing");
            return false;
        }

        var type = typeProperty.GetString();
        if (type.IsNullOrEmpty())
        {
            PluginLog.Warning($"Failed to find [{typeof(T).Name}] builder, required attribute [{TYPE_PROPERTY_NAME}] is null or empty");
            return false;
        }

        if (!Builders.TryGetValue(type, out var builder))
        {
            PluginLog.Warning($"Failed to find [{typeof(T).Name}] builder for type [{type}] (registered types: {string.Join(", ", Builders.Keys)})");
            return false;
        }

        if(!builder.TryBuild(jsonElement, out instance))
        {
            PluginLog.Debug($"Failed to build instance [{typeof(T).Name}] for type [{type}] using builder [{builder.GetType().Name}]");
            return false;
        }

        return true;
    }
}
