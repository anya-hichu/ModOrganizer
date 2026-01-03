using Dalamud.Plugin.Services;
using System;
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
}
