using Dalamud.Plugin.Services;
using Dalamud.Utility;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Elements;

public static class ElementExtensions
{
    private delegate bool TryGetValueFunc<T>(out T value);
    private delegate bool TryGetNullableValueFunc<T>(JsonElement element, [NotNullWhen(true)] out T? value, IPluginLog? maybePluginLog) where T : class?;
    private delegate bool TryGetNotNullableValueFunc<T>(JsonElement element, out T value, IPluginLog? maybePluginLog) where T : notnull;

    public static bool Is(this JsonElement element, JsonValueKind kind, IPluginLog? maybePluginLog = null)
    {
        if (element.ValueKind == kind) return true;
        maybePluginLog?.Warning($"Expected [{kind}] value kind but found [{element.ValueKind}]: {element}");
        return false;
    }

    public static bool HasProperty(this JsonElement element, string name, IPluginLog? maybePluginLog = null)
    {
        if (!element.Is(JsonValueKind.Object, maybePluginLog)) return false;
        return element.TryGetProperty(name, out var _);
    }

    #region Values

    private static void LogNotParsableAs(this JsonElement element, object value, IPluginLog? maybePluginLog = null)
        => maybePluginLog?.Warning($"Expected [{element.ValueKind}] value kind to be parsable as [{value.GetType().Name}]: {element}");

    private static bool TryGetStringValue(this JsonElement element, [NotNullWhen(true)] out string? value, IPluginLog? maybePluginLog = null)
    {
        value = null;
        if (!element.Is(JsonValueKind.String, maybePluginLog)) return false;
        value = element.GetString();
        return value != null;
    }

    private static bool TryGetBoolValue(this JsonElement element, out bool value, IPluginLog? maybePluginLog = null)
    {
        value = default;
        if (element.ValueKind == JsonValueKind.False || element.ValueKind == JsonValueKind.True)
        {
            value = element.GetBoolean();
            return true;
        }
        element.LogNotParsableAs(value, maybePluginLog);
        return false;
    }

    private static bool TryGetNumberValue<T>(this JsonElement element, out T value, TryGetValueFunc<T> func, IPluginLog? maybePluginLog = null) where T : struct
    {
        value = default;
        if (!element.Is(JsonValueKind.Number, maybePluginLog)) return false;
        if (func.Invoke(out value)) return true;
        element.LogNotParsableAs(value, maybePluginLog);
        return false;
    }

    private static bool TryGetDictValue(this JsonElement element, [NotNullWhen(true)] out Dictionary<string, string>? value, IPluginLog? maybePluginLog = null)
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

    private static bool TryGetArrayValue(this JsonElement element, [NotNullWhen(true)] out string[]? value, IPluginLog? maybePluginLog = null)
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

    private static bool TryGetArrayValue(this JsonElement element, [NotNullWhen(true)] out int[]? value, IPluginLog? maybePluginLog = null)
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

    private static bool TryGetNotEmptyValue(this JsonElement element, [NotNullWhen(true)] out string? value, IPluginLog? maybePluginLog = null)
    {
        if (!element.TryGetValue(out value, maybePluginLog)) return false;
        if (value.IsNullOrEmpty())
        {
            maybePluginLog?.Warning("Expected value to not be empty");
            return false;
        }
        return true;
    }

    // https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/meta_enums.json#U8
    private static bool TryGetU8Value(this JsonElement element, out byte value, IPluginLog? maybePluginLog = null)
    {
        if (element.TryGetValue(out value)) return true;
        if (element.TryGetValue(out string? stringValue) && byte.TryParse(stringValue, CultureInfo.InvariantCulture, out value)) return true;
        element.LogNotParsableAs(value, maybePluginLog);
        return false;
    }

    // https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/meta_enums.json#U16
    private static bool TryGetU16Value(this JsonElement element, out ushort value, IPluginLog? maybePluginLog = null)
    {
        if (element.TryGetValue(out value)) return true;
        if (element.TryGetValue(out string? stringValue) && ushort.TryParse(stringValue, CultureInfo.InvariantCulture, out value)) return true;
        element.LogNotParsableAs(value, maybePluginLog);
        return false;
    }

    private static bool TryGetValue(this JsonElement element, [NotNullWhen(true)] out string? value, IPluginLog? maybePluginLog = null) => element.TryGetStringValue(out value, maybePluginLog);
    private static bool TryGetValue(this JsonElement element, out bool value, IPluginLog? maybePluginLog = null) => element.TryGetBoolValue(out value, maybePluginLog);
    private static bool TryGetValue(this JsonElement element, out byte value, IPluginLog? maybePluginLog = null) => element.TryGetNumberValue(out value, element.TryGetByte, maybePluginLog);
    private static bool TryGetValue(this JsonElement element, out ushort value, IPluginLog? maybePluginLog = null) => element.TryGetNumberValue(out value, element.TryGetUInt16, maybePluginLog);
    private static bool TryGetValue(this JsonElement element, out uint value, IPluginLog? maybePluginLog = null) => element.TryGetNumberValue(out value, element.TryGetUInt32, maybePluginLog);
    private static bool TryGetValue(this JsonElement element, out ulong value, IPluginLog? maybePluginLog = null) => element.TryGetNumberValue(out value, element.TryGetUInt64, maybePluginLog);
    private static bool TryGetValue(this JsonElement element, out int value, IPluginLog? maybePluginLog = null) => element.TryGetNumberValue(out value, element.TryGetInt32, maybePluginLog);
    private static bool TryGetValue(this JsonElement element, out long value, IPluginLog? maybePluginLog = null) => element.TryGetNumberValue(out value, element.TryGetInt64, maybePluginLog);
    private static bool TryGetValue(this JsonElement element, out float value, IPluginLog? maybePluginLog = null) => element.TryGetNumberValue(out value, element.TryGetSingle, maybePluginLog);
    private static bool TryGetValue(this JsonElement element, [NotNullWhen(true)] out Dictionary<string, string>? value, IPluginLog? maybePluginLog = null) => element.TryGetDictValue(out value, maybePluginLog);
    private static bool TryGetValue(this JsonElement element, [NotNullWhen(true)] out int[]? value, IPluginLog? maybePluginLog = null) => element.TryGetArrayValue(out value, maybePluginLog);
    private static bool TryGetValue(this JsonElement element, [NotNullWhen(true)] out string[]? value, IPluginLog? maybePluginLog = null) => element.TryGetArrayValue(out value, maybePluginLog);

    #endregion

    #region Optional Property

    public static bool TryGetOptionalProperty(this JsonElement element, string name, out JsonElement property, IPluginLog? maybePluginLog = null)
    {
        property = default;
        if (!element.Is(JsonValueKind.Object, maybePluginLog)) return false;
        if (!element.TryGetProperty(name, out var elementProperty)) return false;
        if (elementProperty.ValueKind == JsonValueKind.Null) return false;
        property = elementProperty;
        return true;
    }

    private static bool TryGetOptionalPropertyValue<T>(this JsonElement element, string propertyName, out T? value, TryGetNullableValueFunc<T> func, IPluginLog? maybePluginLog = null) where T : class
    {
        value = null;
        if (!element.TryGetOptionalProperty(propertyName, out var property, maybePluginLog)) return true;
        return func.Invoke(property, out value, maybePluginLog);
    }

    private static bool TryGetOptionalPropertyValue<T>(this JsonElement element, string propertyName, out T? value, TryGetNotNullableValueFunc<T> func, IPluginLog? maybePluginLog = null) where T : struct
    {
        value = null;
        if (!element.TryGetOptionalProperty(propertyName, out var property, maybePluginLog)) return true;
        if (!func.Invoke(property, out var propertyValue, maybePluginLog)) return false;
        value = propertyValue;
        return true;
    }

    public static bool TryGetOptionalPropertyValue(this JsonElement element, string propertyName, out string? value, IPluginLog? maybePluginLog = null)
        => element.TryGetOptionalPropertyValue(propertyName, out value, TryGetValue, maybePluginLog);

    public static bool TryGetOptionalPropertyValue(this JsonElement element, string propertyName, out bool? value, IPluginLog? maybePluginLog = null) 
        => element.TryGetOptionalPropertyValue(propertyName, out value, TryGetValue, maybePluginLog);

    public static bool TryGetOptionalPropertyValue(this JsonElement element, string propertyName, out ushort? value, IPluginLog? maybePluginLog = null) 
        => element.TryGetOptionalPropertyValue(propertyName, out value, TryGetValue, maybePluginLog);

    public static bool TryGetOptionalPropertyValue(this JsonElement element, string propertyName, out uint? value, IPluginLog? maybePluginLog = null)
        => element.TryGetOptionalPropertyValue(propertyName, out value, TryGetValue, maybePluginLog);

    public static bool TryGetOptionalPropertyValue(this JsonElement element, string propertyName, out int? value, IPluginLog? maybePluginLog = null)
        => element.TryGetOptionalPropertyValue(propertyName, out value, TryGetValue, maybePluginLog);

    public static bool TryGetOptionalPropertyValue(this JsonElement element, string propertyName, out long? value, IPluginLog? maybePluginLog = null)
        => element.TryGetOptionalPropertyValue(propertyName, out value, TryGetValue, maybePluginLog);

    public static bool TryGetOptionalPropertyValue(this JsonElement element, string propertyName, out Dictionary<string, string>? value, IPluginLog? maybePluginLog = null)
        => element.TryGetOptionalPropertyValue(propertyName, out value, TryGetValue, maybePluginLog);

    public static bool TryGetOptionalPropertyValue(this JsonElement element, string propertyName, out string[]? value, IPluginLog? maybePluginLog = null)
        => element.TryGetOptionalPropertyValue(propertyName, out value, TryGetValue, maybePluginLog);

    public static bool TryGetOptionalPropertyValue(this JsonElement element, string propertyName, out int[]? value, IPluginLog? maybePluginLog = null)
        => element.TryGetOptionalPropertyValue(propertyName, out value, TryGetValue, maybePluginLog);

    public static bool TryGetOptionalU16PropertyValue(this JsonElement element, string propertyName, out ushort? value, IPluginLog? maybePluginLog = null)
        => element.TryGetOptionalPropertyValue(propertyName, out value, TryGetU16Value, maybePluginLog);

    #endregion

    #region Required Properties

    public static bool TryGetRequiredProperty(this JsonElement element, string name, out JsonElement property, IPluginLog? maybePluginLog = null)
    {
        property = default;
        if (!element.Is(JsonValueKind.Object, maybePluginLog)) return false;
        if (element.TryGetProperty(name, out property) && property.ValueKind != JsonValueKind.Null) return true;
        maybePluginLog?.Warning($"Expected property [{name}] to be present: {element}");
        return false;
    }

    private static bool TryGetRequiredPropertyValue<T>(this JsonElement element, string propertyName, [NotNullWhen(true)] out T? value, TryGetNullableValueFunc<T> func, IPluginLog? maybePluginLog = null) where T : class
    {
        value = null;
        if (!element.TryGetRequiredProperty(propertyName, out var property, maybePluginLog)) return false;
        return func.Invoke(property, out value, maybePluginLog);
    }

    private static bool TryGetRequiredPropertyValue<T>(this JsonElement element, string propertyName, out T value, TryGetNotNullableValueFunc<T> func, IPluginLog? maybePluginLog = null) where T : struct
    {
        value = default;
        if (!element.TryGetRequiredProperty(propertyName, out var property, maybePluginLog)) return false;
        return func.Invoke(property, out value, maybePluginLog);
    }

    public static bool TryGetRequiredPropertyValue(this JsonElement element, string propertyName, [NotNullWhen(true)] out string? value, IPluginLog? maybePluginLog = null)
        => element.TryGetRequiredPropertyValue(propertyName, out value, TryGetValue, maybePluginLog);

    public static bool TryGetRequiredPropertyValue(this JsonElement element, string propertyName, out bool value, IPluginLog? maybePluginLog = null)
        => element.TryGetRequiredPropertyValue(propertyName, out value, TryGetValue, maybePluginLog);

    public static bool TryGetRequiredPropertyValue(this JsonElement element, string propertyName, out byte value, IPluginLog? maybePluginLog = null)
        => element.TryGetRequiredPropertyValue(propertyName, out value, TryGetValue, maybePluginLog);

    public static bool TryGetRequiredPropertyValue(this JsonElement element, string propertyName, out ushort value, IPluginLog? maybePluginLog = null)
        => element.TryGetRequiredPropertyValue(propertyName, out value, TryGetValue, maybePluginLog);

    public static bool TryGetRequiredPropertyValue(this JsonElement element, string propertyName, out uint value, IPluginLog? maybePluginLog = null)
        => element.TryGetRequiredPropertyValue(propertyName, out value, TryGetValue, maybePluginLog);

    public static bool TryGetRequiredPropertyValue(this JsonElement element, string propertyName, out ulong value, IPluginLog? maybePluginLog = null)
        => element.TryGetRequiredPropertyValue(propertyName, out value, TryGetValue, maybePluginLog);

    public static bool TryGetRequiredPropertyValue(this JsonElement element, string propertyName, out int value, IPluginLog? maybePluginLog = null)
        => element.TryGetRequiredPropertyValue(propertyName, out value, TryGetValue, maybePluginLog);

    public static bool TryGetRequiredPropertyValue(this JsonElement element, string propertyName, out float value, IPluginLog? maybePluginLog = null)
        => element.TryGetRequiredPropertyValue(propertyName, out value, TryGetValue, maybePluginLog);

    public static bool TryGetRequiredPropertyValue(this JsonElement element, string propertyName, [NotNullWhen(true)] out Dictionary<string, string>? value, IPluginLog? maybePluginLog = null)
        => element.TryGetRequiredPropertyValue(propertyName, out value, TryGetValue, maybePluginLog);

    public static bool TryGetRequiredPropertyValue(this JsonElement element, string propertyName, [NotNullWhen(true)] out string[]? value, IPluginLog? maybePluginLog = null)
        => element.TryGetRequiredPropertyValue(propertyName, out value, TryGetValue, maybePluginLog);

    public static bool TryGetRequiredNotEmptyPropertyValue(this JsonElement element, string propertyName, [NotNullWhen(true)] out string? value, IPluginLog? maybePluginLog = null)
        => element.TryGetRequiredPropertyValue(propertyName, out value, TryGetNotEmptyValue, maybePluginLog);

    public static bool TryGetRequiredU8PropertyValue(this JsonElement element, string propertyName, out byte value, IPluginLog? maybePluginLog = null)
        => element.TryGetRequiredPropertyValue(propertyName, out value, TryGetU8Value, maybePluginLog);

    public static bool TryGetRequiredU16PropertyValue(this JsonElement element, string propertyName, out ushort value, IPluginLog? maybePluginLog = null)
        => element.TryGetRequiredPropertyValue(propertyName, out value, TryGetU16Value, maybePluginLog);

    #endregion
}
