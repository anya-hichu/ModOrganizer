using Dalamud.Plugin.Services;
using Scriban.Helpers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;


namespace ModOrganizer.Json;

public class JsonParser(IPluginLog pluginLog)
{
    private static JsonSerializerOptions JsonSerializerOptions { get; set; } = new() { AllowTrailingCommas = true };

    private IPluginLog PluginLog { get; set; } = pluginLog;

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
            PluginLog.Debug($"Failed to parse {typeof(T).ScriptPrettyName()} from json file [{path}] ({e})");
        }
        return false;
    }
}
