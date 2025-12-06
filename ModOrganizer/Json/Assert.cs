using Dalamud.Plugin.Services;
using Dalamud.Utility;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;

namespace ModOrganizer.Json;

public class Assert(IPluginLog pluginLog)
{
    private IPluginLog PluginLog { get; init; } = pluginLog;

    public bool IsValue(JsonElement jsonElement, JsonValueKind kind)
    {
        if (jsonElement.ValueKind == kind) return true;

        PluginLog.Warning($"Expected value kind [{kind}] but got [{jsonElement.ValueKind}]:\n\t{jsonElement}");
        return false;
    }

    public bool IsPropertyPresent(JsonElement jsonElement, string name, out JsonElement property, bool warn = true)
    {
        if (jsonElement.TryGetProperty(name, out property)) return true;

        if (warn) PluginLog.Warning($"Expected property [{name}] is missing:\n\t{jsonElement}");
        return false;
    }

    public bool IsValuePresent(JsonElement jsonElement, string propertyName, [NotNullWhen(true)] out string? value, bool required = true)
    {
        value = null;

        if (!IsPropertyPresent(jsonElement, propertyName, out var property, warn: required)) return false;

        value = property.GetString();
        if (value.IsNullOrEmpty())
        {
            PluginLog.Warning($"Property [{propertyName}] is null or empty:\n\t{property}");
            return false;
        }

        return true;
    }

    // https://github.com/xivdev/Penumbra/blob/master/schemas/structs/meta_enums.json#U8
    public bool IsU8Value(JsonElement jsonElement, string propertyName, out byte value, bool required = true)
    {
        value = default;

        if (!IsPropertyPresent(jsonElement, propertyName, out var property, warn: required)) return false;

        if (property.ValueKind == JsonValueKind.Number && property.TryGetByte(out value)) return true;
        if (property.ValueKind == JsonValueKind.String && byte.TryParse(property.GetString(), CultureInfo.InvariantCulture, out value)) return true;

        PluginLog.Warning($"Property [{propertyName}] is not parsable as [{typeof(byte).Name}]:\n\t{property}");
        return false;
    }

    // https://github.com/xivdev/Penumbra/blob/master/schemas/structs/meta_enums.json#U16
    public bool IsU16Value(JsonElement jsonElement, string propertyName, out ushort value, bool required = true)
    {
        value = default;

        if (!IsPropertyPresent(jsonElement, propertyName, out var property, warn: required)) return false;

        if (property.ValueKind == JsonValueKind.Number && property.TryGetUInt16(out value)) return true;
        if (property.ValueKind == JsonValueKind.String && ushort.TryParse(property.GetString(), CultureInfo.InvariantCulture, out value)) return true;

        PluginLog.Warning($"Property [{propertyName}] is not parsable as [{typeof(ushort).Name}]:\n\t{property}");
        return false;
    }

    public bool IsStringDict(JsonElement jsonElement, [NotNullWhen(true)] out Dictionary<string, string>? value)
    {
        value = null;

        if (!IsValue(jsonElement, JsonValueKind.Object)) return false;

        var dict = new Dictionary<string, string>();
        foreach (var item in jsonElement.EnumerateObject())
        {
            if (!IsValue(item.Value, JsonValueKind.String)) return false;
            dict.Add(item.Name, item.Value.GetString()!);
        }

        value = dict;
        return true;
    }

    public bool IsStringArray(JsonElement jsonElement, [NotNullWhen(true)] out string[]? value)
    {
        value = null;

        if (!IsValue(jsonElement, JsonValueKind.Array)) return false;

        var list = new List<string>();
        foreach (var item in jsonElement.EnumerateArray())
        {
            if (!IsValue(item, JsonValueKind.String)) return false;
            list.Add(item.GetString()!);
        }

        value = [.. list];
        return true;
    }

    public bool IsIntArray(JsonElement jsonElement, [NotNullWhen(true)] out int[]? value)
    {
        value = null;

        if (!IsValue(jsonElement, JsonValueKind.Array)) return false;

        var list = new List<int>();
        foreach (var item in jsonElement.EnumerateArray())
        {
            if (!item.TryGetInt32(out var parsed)) return false;
            list.Add(parsed);
        }

        value = [.. list];
        return true;
    }
}
