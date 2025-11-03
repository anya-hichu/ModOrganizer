using Dalamud.Plugin.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ModOrganizer.Utils;

public class ConfigParser(IPluginLog pluginLog)
{

    private IPluginLog PluginLog { get; init; } = pluginLog;

    public Dictionary<string, object?> ParseFile(string path)
    {
        if (!Path.Exists(path))
        {
            PluginLog.Debug($"Failed to find json file [{path}], returning empty");
            return [];
        }

        using var reader = new StreamReader(path);
        var json = reader.ReadToEnd();

        try
        {
            if (Deserialize(json) is not Dictionary<string, object?> data)
            {
                PluginLog.Debug($"Failed to parse json file [{path}], returning empty");
                return [];
            }

            return data;
        }
        catch (JsonException e)
        {
            PluginLog.Debug($"Failed to parse json file [{path}] ({e.Message}), returning empty");
            return [];
        }
    }

    public static object? Deserialize(string json) => Convert(JsonSerializer.Deserialize<JsonElement>(json, options: new() { AllowTrailingCommas = true }));

    private static object? Convert(JsonElement element) =>
        element.ValueKind switch
        {
            JsonValueKind.Object => ConvertObject(element),
            JsonValueKind.Array => ConvertArray(element),
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number when element.TryGetInt64(out var l) => l,
            JsonValueKind.Number => element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            _ => null
        };

    private static Dictionary<string, object?> ConvertObject(JsonElement element) => element.EnumerateObject().ToDictionary(p => p.Name, p => Convert(p.Value));
    private static List<object?> ConvertArray(JsonElement element) => [.. element.EnumerateArray().Select(Convert)];
}
