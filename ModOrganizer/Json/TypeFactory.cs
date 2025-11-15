using Dalamud.Plugin.Services;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json;

public abstract class TypeFactory<T>(IPluginLog pluginLog) : Factory<T>(pluginLog) where T : class
{
    private static readonly string TYPE_PROPERTY_NAME = "Type";

    protected Dictionary<string, Builder<T>> Builders { get; init; } = [];

    protected override bool TryGetBuilder(JsonElement jsonElement, [NotNullWhen(true)] out Builder<T>? builder)
    {
        builder = null;

        if (!AssertObject(jsonElement)) return false;

        if (!AssertPropertyValuePresent(jsonElement, TYPE_PROPERTY_NAME, out var type)) return false;

        if (!Builders.TryGetValue(type, out builder))
        {
            PluginLog.Warning($"Failed to find [{typeof(T).Name}] builder for type [{type}] (registered types: {string.Join(", ", Builders.Keys)}):\n\t{jsonElement}");
            return false;
        }

        return true;
    }
}
