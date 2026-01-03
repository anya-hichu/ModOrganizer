using Dalamud.Plugin.Services;
using System;
using System.IO;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Elements;

public class ElementReader(IPluginLog pluginLog) : IElementReader
{
    // Some mods use trailing commas for some reasons
    private JsonSerializerOptions Options { get; init; } = new() { AllowTrailingCommas = true };

    public bool TryReadFromData(string data, out JsonElement instance)
    {
        instance = default;

        try
        {
            instance = JsonSerializer.Deserialize<JsonElement>(data, Options);

            return true;
        }
        catch (Exception e)
        {
            pluginLog.Error($"Caught exception while reading [{nameof(JsonElement)}] from data ({e.Message}): {data}");
        }

        return false;
    }

    public bool TryReadFromFile(string path, out JsonElement instance)
    {
        instance = default;

        try
        {
            var data = File.ReadAllText(path);

            if (TryReadFromData(data, out instance)) return true;

            pluginLog.Warning($"Failed to read [{nameof(JsonElement)}] from json file [{path}]");
        }
        catch (Exception e)
        {
            pluginLog.Error($"Caught exception while reading [{nameof(JsonElement)}] from json file [{path}] ({e.Message})");
        }

        return false;
    } 
}
