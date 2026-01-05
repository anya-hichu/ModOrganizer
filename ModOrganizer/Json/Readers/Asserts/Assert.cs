using Dalamud.Plugin.Services;
using Dalamud.Utility;
using ModOrganizer.Json.Penumbra.Containers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Asserts;

public class Assert(IPluginLog pluginLog) : IAssert
{
    public bool IsValue(JsonElement element, JsonValueKind kind)
    {
        if (element.ValueKind == kind) return true;

        pluginLog.Warning($"Expected value kind [{kind}] but found [{element.ValueKind}]: {element}");
        return false;
    }

    public bool IsPropertyPresent(JsonElement element, string name, out JsonElement property, bool warn = true)
    {
        if (element.TryGetProperty(name, out property)) return true;

        if (warn) pluginLog.Warning($"Expected property [{name}] is missing: {element}");
        return false;
    }

    public bool IsValuePresent(JsonElement element, string propertyName, [NotNullWhen(true)] out string? value, bool required = true)
    {
        value = null;

        if (!IsPropertyPresent(element, propertyName, out var property, warn: required)) return false;

        value = property.GetString();
        if (value.IsNullOrEmpty())
        {
            pluginLog.Warning($"Property [{propertyName}] is null or empty: {element}");
            return false;
        }

        return true;
    }

    public bool IsOptionalValue(JsonElement element, string propertyName, out string? value)
    {
        value = null;

        switch (element.TryGetProperty(propertyName, out var property))
        {
            case true when property.ValueKind == JsonValueKind.String:
                value = property.GetString();
                return true;
            case true when property.ValueKind == JsonValueKind.Null:
            case false: return true;
            default:
                pluginLog.Warning($"Expected property [{nameof(NamedContainer.Name)}] value kind to be [{JsonValueKind.String}] or [{JsonValueKind.Null}] but found [{property.ValueKind}]: {element}");
                break;
        }

        return false;
    }

    // https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/meta_enums.json#U8
    public bool IsU8Value(JsonElement element, string propertyName, out byte value, bool required = true)
    {
        value = default;

        if (!IsPropertyPresent(element, propertyName, out var property, warn: required)) return false;

        if (property.ValueKind == JsonValueKind.Number && property.TryGetByte(out value)) return true;
        if (property.ValueKind == JsonValueKind.String && byte.TryParse(property.GetString(), CultureInfo.InvariantCulture, out value)) return true;

        pluginLog.Warning($"Property [{propertyName}] is not parsable as [{typeof(byte).Name}]: {element}");
        return false;
    }

    // https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/meta_enums.json#U16
    public bool IsU16Value(JsonElement element, string propertyName, out ushort value, bool required = true)
    {
        value = default;

        if (!IsPropertyPresent(element, propertyName, out var property, warn: required)) return false;

        if (property.ValueKind == JsonValueKind.Number && property.TryGetUInt16(out value)) return true;
        if (property.ValueKind == JsonValueKind.String && ushort.TryParse(property.GetString(), CultureInfo.InvariantCulture, out value)) return true;

        pluginLog.Warning($"Property [{propertyName}] is not parsable as [{typeof(ushort).Name}]: {element}");
        return false;
    }

    public bool IsStringDict(JsonElement element, [NotNullWhen(true)] out Dictionary<string, string>? value)
    {
        value = null;

        if (!IsValue(element, JsonValueKind.Object)) return false;

        var dict = new Dictionary<string, string>();
        foreach (var item in element.EnumerateObject())
        {
            if (!IsValue(item.Value, JsonValueKind.String)) return false;
            dict.Add(item.Name, item.Value.GetString()!);
        }

        value = dict;
        return true;
    }

    public bool IsStringArray(JsonElement element, [NotNullWhen(true)] out string[]? value)
    {
        value = null;

        if (!IsValue(element, JsonValueKind.Array)) return false;

        var list = new List<string>();
        foreach (var item in element.EnumerateArray())
        {
            if (!IsValue(item, JsonValueKind.String)) return false;
            list.Add(item.GetString()!);
        }

        value = [.. list];
        return true;
    }

    public bool IsIntArray(JsonElement element, [NotNullWhen(true)] out int[]? value)
    {
        value = null;

        if (!IsValue(element, JsonValueKind.Array)) return false;

        var list = new List<int>();
        foreach (var item in element.EnumerateArray())
        {
            if (!IsValue(item, JsonValueKind.Number)) return false;
            if (!item.TryGetInt32(out var parsed)) return false;
            list.Add(parsed);
        }

        value = [.. list];
        return true;
    }
}
