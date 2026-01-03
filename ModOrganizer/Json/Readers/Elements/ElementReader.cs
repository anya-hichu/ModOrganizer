using Dalamud.Plugin.Services;
using System;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Elements;

public class ElementReader(IPluginLog pluginLog) : IElementReader
{
    private JsonSerializerOptions Options { get; init; } = new() { AllowTrailingCommas = true };

    public bool TryReadFromData(string data, out JsonElement instance)
    {
        instance = default;

        try
        {
            instance = JsonSerializer.SerializeToElement(data, Options);
            return true;
        }
        catch (Exception e)
        {
            pluginLog.Error($"Caught exception while converting data to json element  ({e.Message}): [{data}]");
        }

        return false;
    }
}
