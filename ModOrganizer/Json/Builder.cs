using Dalamud.Plugin.Services;
using Dalamud.Utility;
using ModOrganizer.Json.Imcs;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json;

public abstract class Builder<T>(IPluginLog pluginLog)
{
    public IPluginLog PluginLog { get; set; } = pluginLog;

    public abstract bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out T? instance);

    #region Asserts
    protected bool AssertIsObject(JsonElement jsonElement)
    {
        if (jsonElement.ValueKind == JsonValueKind.Object) return true;

        PluginLog.Warning($"Failed to build [{typeof(T).Name}], expected root object but got [{jsonElement.ValueKind}]");
        return false;
    }

    protected bool AssertHasProperty(JsonElement jsonElement, string name, [NotNullWhen(true)] out JsonElement property)
    {
        if (jsonElement.TryGetProperty(name, out property)) return true;

        PluginLog.Warning($"Failed to build [{typeof(T).Name}], required attribute [{name}] is missing");
        return false;
    }

    protected bool AssertStringPropertyPresent(JsonElement jsonElement, string name, [NotNullWhen(true)] out string? value)
    {
        value = default;

        if (!AssertHasProperty(jsonElement, name, out var property)) return false;

        value = property.GetString();
        if (value.IsNullOrWhitespace())
        {
            PluginLog.Warning($"Failed to build [{typeof(T).Name}], required attribute [{name}] is null or empty");
            return false;
        }

        return true;
    }
    #endregion
}
