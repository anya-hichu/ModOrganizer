using Dalamud.Plugin.Services;
using Dalamud.Utility;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json;

public abstract class TypeFactory<T>(IPluginLog pluginLog) : Factory<T>(pluginLog)
{
    private static readonly string TYPE_PROPERTY_NAME = "Type";

    protected Dictionary<string, Builder<T>> Builders { get; init; } = [];

    protected override bool TryGetBuilder(JsonElement jsonElement, [NotNullWhen(true)] out Builder<T>? builder)
    {
        builder = default;

        if (!AssertIsObject(jsonElement)) return false;

        if (!AssertStringPropertyPresent(jsonElement, TYPE_PROPERTY_NAME, out var type)) return false;

        if (!Builders.TryGetValue(type, out builder))
        {
            PluginLog.Warning($"Failed to find [{typeof(T).Name}] builder for type [{type}] (registered types: {string.Join(", ", Builders.Keys)})");
            return false;
        }

        return true;
    }
}
