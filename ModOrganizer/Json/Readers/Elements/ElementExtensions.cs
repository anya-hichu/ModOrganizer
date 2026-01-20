using Dalamud.Plugin.Services;
using Dalamud.Utility;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Elements;

public static class ElementExtensions
{
    public static bool Is(this JsonElement element, JsonValueKind kind, IPluginLog? maybePluginLog = null)
    {
        if (element.ValueKind == kind) return true;
        maybePluginLog?.Warning($"Expected [{kind}] but found [{element.ValueKind}]: {element}");
        return false;
    }

    #region Values

    public static bool TryGetValue(this JsonElement element, [NotNullWhen(true)] out string? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!element.Is(JsonValueKind.String, maybePluginLog)) return false;

        value = element.GetString();
        return value != null;
    }

    public static bool TryGetValue(this JsonElement element, out bool value, IPluginLog? maybePluginLog = null)
    {
        value = default;
        if (!element.Is(JsonValueKind.False, maybePluginLog) && !element.Is(JsonValueKind.True, maybePluginLog)) return false;
        value = element.GetBoolean();
        return true;
    }

    public static bool TryGetValue(this JsonElement element, out byte value, IPluginLog? maybePluginLog = null)
    {
        value = default;
        if (!element.Is(JsonValueKind.Number, maybePluginLog)) return false;
        return element.TryGetByte(out value);
    }

    public static bool TryGetValue(this JsonElement element, out ushort value, IPluginLog? maybePluginLog = null)
    {
        value = default;
        if (!element.Is(JsonValueKind.Number, maybePluginLog)) return false;
        return element.TryGetUInt16(out value);
    }

    public static bool TryGetValue(this JsonElement element, out uint value, IPluginLog? maybePluginLog = null)
    {
        value = default;
        if (!element.Is(JsonValueKind.Number, maybePluginLog)) return false;
        return element.TryGetUInt32(out value);
    }

    public static bool TryGetValue(this JsonElement element, out ulong value, IPluginLog? maybePluginLog = null)
    {
        value = default;
        if (!element.Is(JsonValueKind.Number, maybePluginLog)) return false;
        return element.TryGetUInt64(out value);
    }

    public static bool TryGetValue(this JsonElement element, out int value, IPluginLog? maybePluginLog = null)
    {
        value = default;
        if (!element.Is(JsonValueKind.Number, maybePluginLog)) return false;
        return element.TryGetInt32(out value);
    }

    public static bool TryGetValue(this JsonElement element, out long value, IPluginLog? maybePluginLog = null)
    {
        value = default;
        if (!element.Is(JsonValueKind.Number, maybePluginLog)) return false;
        return element.TryGetInt64(out value);
    }

    public static bool TryGetValue(this JsonElement element, out float value, IPluginLog? maybePluginLog = null)
    {
        value = default;
        if (!element.Is(JsonValueKind.Number, maybePluginLog)) return false;
        return element.TryGetSingle(out value);
    }

    public static bool TryGetValue(this JsonElement element, [NotNullWhen(true)] out Dictionary<string, string>? value, IPluginLog? maybePluginLog = null)
    {
        value = null;

        if (!element.Is(JsonValueKind.Object, maybePluginLog)) return false;

        var dict = new Dictionary<string, string>();
        foreach (var item in element.EnumerateObject())
        {
            if (!item.Value.TryGetValue(out string? itemValue, maybePluginLog)) return false;
            dict.Add(item.Name, itemValue);
        }
        value = dict;
        return true;
    }

    public static bool TryGetValue(this JsonElement element, [NotNullWhen(true)] out string[]? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!element.Is(JsonValueKind.Array, maybePluginLog)) return false;
        var list = new List<string>();
        foreach (var item in element.EnumerateArray())
        {
            if (!item.TryGetValue(out string? itemValue, maybePluginLog)) return false;
            list.Add(itemValue);
        }
        value = [.. list];
        return true;
    }

    public static bool TryGetValue(this JsonElement element, [NotNullWhen(true)] out int[]? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!element.Is(JsonValueKind.Array, maybePluginLog)) return false;
        var list = new List<int>();
        foreach (var item in element.EnumerateArray())
        {
            if (!item.TryGetValue(out int itemValue, maybePluginLog)) return false;
            list.Add(itemValue);
        }
        value = [.. list];
        return true;
    }

    public static bool TryGetNotEmptyValue(this JsonElement element, [NotNullWhen(true)] out string? value, IPluginLog? maybePluginLog = null)
    {
        if (!element.TryGetValue(out value, maybePluginLog)) return false;
        if (value.IsNullOrEmpty())
        {
            maybePluginLog?.Warning($"Expected value to not be empty: {element}");
            return false;
        }
        return true;
    }

    // https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/meta_enums.json#U8
    public static bool TryGetU8Value(this JsonElement element, out byte value, IPluginLog? maybePluginLog = null)
    {
        value = default;
        if ((element.ValueKind == JsonValueKind.Number && element.TryGetByte(out var parsedValue)) || (element.ValueKind == JsonValueKind.String && byte.TryParse(element.GetString(), CultureInfo.InvariantCulture, out parsedValue)))
        {
            value = parsedValue;
            return true;
        }
        maybePluginLog?.Warning($"Expected value kind [{element.ValueKind}] to be parsable as U8: {element}");
        return false;
    }

    // https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/meta_enums.json#U16
    public static bool TryGetU16Value(this JsonElement element, out ushort value, IPluginLog? maybePluginLog = null)
    {
        value = default;
        if ((element.ValueKind == JsonValueKind.Number && element.TryGetUInt16(out var parsedValue)) || (element.ValueKind == JsonValueKind.String && ushort.TryParse(element.GetString(), CultureInfo.InvariantCulture, out parsedValue)))
        {
            value = parsedValue;
            return true;
        }
        maybePluginLog?.Warning($"Expected value kind [{element.ValueKind}] to be parsable as U16: {element}");
        return false;
    }

    #endregion

    public static bool TryGetProperty(this JsonElement element, string name, out JsonElement property, IPluginLog? maybePluginLog = null, bool required = false)
    {
        if (element.TryGetProperty(name, out property) && (!required || !property.Is(JsonValueKind.Null))) return true;

        maybePluginLog?.Warning($"Expected property [{name}] to be present: {element}");
        return false;
    }

    #region Optional Property Value

    public static bool TryGetOptionalPropertyValue(this JsonElement element, string propertyName, out string? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!element.TryGetProperty(propertyName, out var property, maybePluginLog, required: false)) return true;
        return property.TryGetValue(out value, maybePluginLog);
    }

    public static bool TryGetOptionalPropertyValue(this JsonElement element, string propertyName, out bool? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!TryGetProperty(element, propertyName, out var property, maybePluginLog, required: false)) return true;
        if (!property.TryGetValue(out bool propertyValue, maybePluginLog)) return false;
        value = propertyValue;
        return true;
    }

    public static bool TryGetOptionalPropertyValue(this JsonElement element, string propertyName, out byte? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!TryGetProperty(element, propertyName, out var property, maybePluginLog, required: false)) return true;
        if (!property.TryGetValue(out byte propertyValue, maybePluginLog)) return false;
        value = propertyValue;
        return true;
    }

    public static bool TryGetOptionalPropertyValue(this JsonElement element, string propertyName, out ushort? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!TryGetProperty(element, propertyName, out var property, maybePluginLog, required: false)) return true;
        if (!property.TryGetValue(out ushort propertyValue, maybePluginLog)) return false;
        value = propertyValue;
        return true;
    }

    public static bool TryGetOptionalPropertyValue(this JsonElement element, string propertyName, out uint? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!TryGetProperty(element, propertyName, out var property, maybePluginLog, required: false)) return true;
        if (!property.TryGetValue(out uint propertyValue, maybePluginLog)) return false;
        value = propertyValue;
        return true;
    }

    public static bool TryGetOptionalPropertyValue(this JsonElement element, string propertyName, out ulong? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!TryGetProperty(element, propertyName, out var property, maybePluginLog, required: false)) return true;
        if (!property.TryGetValue(out ulong propertyValue, maybePluginLog)) return false;
        value = propertyValue;
        return true;
    }

    public static bool TryGetOptionalPropertyValue(this JsonElement element, string propertyName, out int? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!TryGetProperty(element, propertyName, out var property, maybePluginLog, required: false)) return true;
        if (!property.TryGetValue(out int propertyValue, maybePluginLog)) return false;
        value = propertyValue;
        return true;
    }

    public static bool TryGetOptionalPropertyValue(this JsonElement element, string propertyName, out long? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!TryGetProperty(element, propertyName, out var property, maybePluginLog, required: false)) return true;
        if (!property.TryGetValue(out long propertyValue, maybePluginLog)) return false;
        value = propertyValue;
        return true;
    }

    public static bool TryGetOptionalPropertyValue(this JsonElement element, string propertyName, out float? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!TryGetProperty(element, propertyName, out var property, maybePluginLog, required: false)) return true;
        if (!property.TryGetValue(out float propertyValue, maybePluginLog)) return false;
        value = propertyValue;
        return true;
    }

    public static bool TryGetOptionalPropertyValue(this JsonElement element, string propertyName, out Dictionary<string, string>? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!TryGetProperty(element, propertyName, out var property, maybePluginLog, required: false)) return true;
        return property.TryGetValue(out value, maybePluginLog);
    }

    public static bool TryGetOptionalPropertyValue(this JsonElement element, string propertyName, out string[]? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!TryGetProperty(element, propertyName, out var property, maybePluginLog, required: false)) return true;
        return TryGetValue(property, out value, maybePluginLog);
    }

    public static bool TryGetOptionalPropertyValue(this JsonElement element, string propertyName, out int[]? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!TryGetProperty(element, propertyName, out var property, maybePluginLog, required: false)) return true;
        return TryGetValue(property, out value, maybePluginLog);
    }

    public static bool TryGetOptionalPropertyU8Value(this JsonElement element, string propertyName, out byte? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!TryGetProperty(element, propertyName, out var property, maybePluginLog, required: false)) return true;
        if (!property.TryGetU8Value(out var propertyValue, maybePluginLog)) return false;
        value = propertyValue;
        return true;
    }

    public static bool TryGetOptionalPropertyU16Value(this JsonElement element, string propertyName, out ushort? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!TryGetProperty(element, propertyName, out var property, maybePluginLog, required: false)) return true;
        if (!property.TryGetU16Value(out var propertyValue, maybePluginLog)) return false;
        value = propertyValue;
        return true;
    }

    #endregion

    #region Required Property Value

    public static bool TryGetRequiredPropertyValue(this JsonElement element, string propertyName, [NotNullWhen(true)] out string? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!element.TryGetProperty(propertyName, out var property, maybePluginLog, required: true)) return false;
        return property.TryGetValue(out value, maybePluginLog);
    }

    public static bool TryGetRequiredPropertyValue(this JsonElement element, string propertyName, out bool value, IPluginLog? maybePluginLog = null)
    {
        value = default;
        if (!TryGetProperty(element, propertyName, out var property, maybePluginLog, required: true)) return false;
        return property.TryGetValue(out value, maybePluginLog);
    }

    public static bool TryGetRequiredPropertyValue(this JsonElement element, string propertyName, out byte value, IPluginLog? maybePluginLog = null)
    {
        value = default;
        if (!element.TryGetProperty(propertyName, out var property, maybePluginLog, required: true)) return false;
        return property.TryGetValue(out value, maybePluginLog);
    }

    public static bool TryGetRequiredPropertyValue(this JsonElement element, string propertyName, out ushort value, IPluginLog? maybePluginLog = null)
    {
        value = default;
        if (!element.TryGetProperty(propertyName, out var property, maybePluginLog, required: true)) return false;
        return property.TryGetValue(out value, maybePluginLog);
    }

    public static bool TryGetRequiredPropertyValue(this JsonElement element, string propertyName, out uint value, IPluginLog? maybePluginLog = null)
    {
        value = default;
        if (!TryGetProperty(element, propertyName, out var property, maybePluginLog, required: true)) return false;
        return property.TryGetValue(out value, maybePluginLog);
    }

    public static bool TryGetRequiredPropertyValue(this JsonElement element, string propertyName, out ulong value, IPluginLog? maybePluginLog = null)
    {
        value = default;
        if (!TryGetProperty(element, propertyName, out var property, maybePluginLog, required: true)) return false;
        return property.TryGetValue(out value, maybePluginLog);
    }

    public static bool TryGetRequiredPropertyValue(this JsonElement element, string propertyName, out int value, IPluginLog? maybePluginLog = null)
    {
        value = default;
        if (!element.TryGetProperty(propertyName, out var property, maybePluginLog, required: true)) return false;
        return property.TryGetValue(out value, maybePluginLog);
    }

    public static bool TryGetRequiredPropertyValue(this JsonElement element, string propertyName, out float value, IPluginLog? maybePluginLog = null)
    {
        value = default;
        if (!element.TryGetProperty(propertyName, out var property, maybePluginLog, required: true)) return false;
        return property.TryGetValue(out value, maybePluginLog);
    }

    public static bool TryGetRequiredPropertyValue(this JsonElement element, string propertyName, [NotNullWhen(true)] out Dictionary<string, string>? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!TryGetProperty(element, propertyName, out var property, maybePluginLog, required: true)) return false;
        return TryGetValue(property, out value);
    }

    public static bool TryGetRequiredPropertyValue(this JsonElement element, string propertyName, [NotNullWhen(true)] out string[]? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!TryGetProperty(element, propertyName, out var property, maybePluginLog, required: true)) return false;
        return TryGetValue(property, out value);
    }

    public static bool TryGetRequiredPropertyValue(this JsonElement element, string propertyName, [NotNullWhen(true)] out int[]? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!TryGetProperty(element, propertyName, out var property, maybePluginLog, required: true)) return false;
        return TryGetValue(property, out value);
    }

    public static bool TryGetRequiredU8PropertyValue(this JsonElement element, string propertyName, out byte value, IPluginLog? maybePluginLog = null)
    {
        value = default;
        if (!element.TryGetProperty(propertyName, out var property, maybePluginLog, required: true)) return false;
        return property.TryGetU8Value(out value, maybePluginLog);
    }

    public static bool TryGetRequiredU16PropertyValue(this JsonElement element, string propertyName, out ushort value, IPluginLog? maybePluginLog = null)
    {
        value = default;
        if (!element.TryGetProperty(propertyName, out var property, maybePluginLog, required: true)) return false;
        return property.TryGetU16Value(out value, maybePluginLog);
    }

    public static bool TryGetRequiredNotEmptyPropertyValue(this JsonElement element, string propertyName, [NotNullWhen(true)] out string? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!element.TryGetProperty(propertyName, out var property, maybePluginLog)) return false;
        return property.TryGetNotEmptyValue(out value, maybePluginLog);
    }

    #endregion
}
