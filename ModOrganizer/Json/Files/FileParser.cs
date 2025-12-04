using Dalamud.Plugin.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;

namespace ModOrganizer.Json.Files;

public class FileParser(IPluginLog pluginLog)
{
    // Some mods use tailing commas for some reason
    private JsonSerializerOptions Options { get; init; } = new() { AllowTrailingCommas = true };

    private IPluginLog PluginLog { get; init; } = pluginLog;

    public bool TryParseFile<T>(string path, [NotNullWhen(true)] out T? parsed)
    {
        parsed = default;
        try
        {
            var json = File.ReadAllText(path);
            parsed = JsonSerializer.Deserialize<T>(json, Options);
            if (parsed != null) return true;
            PluginLog.Error($"Failed to parse [{typeof(T).Name}] from json file [{path}]");
        }
        catch (Exception e)
        {
            PluginLog.Error($"Caught exception while parsing [{typeof(T).Name}] from json file [{path}] ({e})");
        }
        return false;
    }
}
