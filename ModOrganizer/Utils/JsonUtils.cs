using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ModOrganizer.Utils;

public static class JsonUtils
{
    public static object? DeserializeToDynamic(string json) => Convert(JsonSerializer.Deserialize<JsonElement>(json, options: new() { AllowTrailingCommas = true }));

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
