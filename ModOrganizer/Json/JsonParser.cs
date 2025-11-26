using Dalamud.Plugin.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;

namespace ModOrganizer.Json;

public class JsonParser(IPluginLog pluginLog)
{
    // Some mods use tailing commas for some reason
    private JsonSerializerOptions JsonSerializerOptions { get; init; } = new() { AllowTrailingCommas = true };

    private IPluginLog PluginLog { get; init; } = pluginLog;

    public bool TryParseFile<T>(string path, [NotNullWhen(true)] out T? parsed)
    {
        parsed = default;
        try
        {
            var json = File.ReadAllText(path);
            parsed = JsonSerializer.Deserialize<T>(json, JsonSerializerOptions);
            if (parsed != null) return true;
        }
        catch (Exception e)
        {
            PluginLog.Error($"Failed to parse [{typeof(T).Name}] from json file [{path}] ({e})");
        }
        return false;
    }
}
