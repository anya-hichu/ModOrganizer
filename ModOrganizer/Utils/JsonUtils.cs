using System.Collections.Generic;
using System.Text.Json;

namespace ModOrganizer.Utils;

public static class JsonUtils
{
    public static object? DeserializeToDynamic(string json)
    {
        var element = JsonSerializer.Deserialize<JsonElement>(json);
        return Convert(element);
    }

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

    private static Dictionary<string, object?> ConvertObject(JsonElement element)
    {
        var dict = new Dictionary<string, object?>();
        foreach (var prop in element.EnumerateObject())
            dict[prop.Name] = Convert(prop.Value);
        return dict;
    }

    private static List<object?> ConvertArray(JsonElement element)
    {
        var list = new List<object?>();
        foreach (var item in element.EnumerateArray())
            list.Add(Convert(item));
        return list;
    }
}
