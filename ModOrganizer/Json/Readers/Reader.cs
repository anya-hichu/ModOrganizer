using Dalamud.Plugin.Services;
using Dalamud.Utility;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;

namespace ModOrganizer.Json.Readers;

public abstract class Reader<T>(IPluginLog pluginLog) : IReader<T> where T : class
{
    public IPluginLog PluginLog { get; init; } = pluginLog;

    public abstract bool TryRead(JsonElement element, [NotNullWhen(true)] out T? instance);

    public bool TryReadMany(JsonElement element, [NotNullWhen(true)] out T[]? instances)
    {
        instances = null;
        if (!IsValue(element, JsonValueKind.Array)) return false;

        var list = new List<T>();
        foreach (var item in element.EnumerateArray())
        {
            if (!TryRead(item, out var instance))
            {
                PluginLog.Debug($"Failed to read [{typeof(T).Name}]: {item}");
                return false;
            }
            list.Add(instance);
        }

        instances = [.. list];
        return true;
    }

    protected bool IsValue(JsonElement element, JsonValueKind kind)
    {
        if (element.ValueKind == kind) return true;

        PluginLog.Warning($"Expected value kind [{kind}] but found [{element.ValueKind}]: {element}");
        return false;
    }

    protected bool TryGetRequiredProperty(JsonElement element, string name, out JsonElement property)
    {
        if (element.TryGetProperty(name, out property)) return true;

        PluginLog.Warning($"Expected [{typeof(T).Name}] property [{name}] is missing for [{typeof(T).Name}]: {element}");
        return false;
    }

    protected bool TryGetRequiredValue(JsonElement element, string propertyName, [NotNullWhen(true)] out string? value)
    {
        value = null;

        if (!TryGetRequiredProperty(element, propertyName, out var property)) return false;
        if (!IsValue(property, JsonValueKind.String)) return false;

        var parsedValue = property.GetString();
        if (parsedValue.IsNullOrEmpty())
        {
            PluginLog.Warning($"Expected [{typeof(T).Name}] property [{propertyName}] value to not be null or empty: {element}");
            return false;
        }

        value = parsedValue;
        return true;
    }

    protected bool TryGetOptionalValue(JsonElement element, string propertyName, out string? value)
    {
        value = null;

        switch (element.TryGetProperty(propertyName, out var property))
        {
            case true when property.ValueKind == JsonValueKind.String:
                value = property.GetString();
                return true;
            case true when property.ValueKind == JsonValueKind.Null:
            case false:
                return true;
            default:
                PluginLog.Warning($"Expected [{typeof(T).Name}] optional property [{propertyName}] value to be [{JsonValueKind.String}] but found kind [{property.ValueKind}]: {element}");
                break;
        }

        return false;
    }

    protected bool TryGetOptionalValue(JsonElement element, string propertyName, out int? value)
    {
        value = null;

        switch (element.TryGetProperty(propertyName, out var property))
        {
            case true when property.ValueKind == JsonValueKind.Number && property.TryGetInt32(out var parsedValue):
                value = parsedValue;
                return true;
            case true when property.ValueKind == JsonValueKind.Null:
            case false:
                return true;
            default:
                PluginLog.Warning($"Expected [{typeof(T).Name}] optional property [{propertyName}] value to be parsable as [{typeof(int).Name}] but found kind [{property.ValueKind}]: {element}");
                break;
        }

        return false;
    }

    // https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/meta_enums.json#U8
    private static bool TryGetU8(JsonElement element, out byte value)
    {
        value = default;

        if ((element.ValueKind == JsonValueKind.Number && element.TryGetByte(out var parsedValue)) || (element.ValueKind == JsonValueKind.String && byte.TryParse(element.GetString(), CultureInfo.InvariantCulture, out parsedValue)))
        {
            value = parsedValue;
            return true;
        }

        return false;
    }

    protected bool TryGetOptionalU8Value(JsonElement element, string propertyName, out byte value)
    {
        value = default;

        if (!element.TryGetProperty(propertyName, out var property) || property.ValueKind == JsonValueKind.Null) return true;
        if (!TryGetU8(property, out value))
        {
            PluginLog.Warning($"Expected [{typeof(T).Name}] optional property [{propertyName}] value to be parsable as [{typeof(byte).Name}] but found [{property.ValueKind}]: {element}");
            return false;
        }

        return true;
    }

    protected bool TryGetRequiredU8Value(JsonElement element, string propertyName, out byte value)
    {
        value = default;

        if (!TryGetRequiredProperty(element, propertyName, out var property)) return false;
        if (!TryGetU8(property, out value))
        {
            PluginLog.Warning($"Expected [{typeof(T).Name}] property [{propertyName}] value to be parsable as [{typeof(byte).Name}] but found [{property.ValueKind}]: {element}");
            return false;
        }

        return true;
    }

    // https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/meta_enums.json#U16
    private static bool TryGetU16(JsonElement element, out ushort value)
    {
        value = default;

        if ((element.ValueKind == JsonValueKind.Number && element.TryGetUInt16(out var parsedValue)) || (element.ValueKind == JsonValueKind.String && ushort.TryParse(element.GetString(), CultureInfo.InvariantCulture, out parsedValue)))
        {
            value = parsedValue;
            return true;
        }

        return false;
    }

    protected bool TryGetOptionalU16Value(JsonElement element, string propertyName, out ushort value)
    {
        value = default;

        if (!element.TryGetProperty(propertyName, out var property) || property.ValueKind == JsonValueKind.Null) return true;
        if (TryGetU16(property, out value)) return true;

        PluginLog.Warning($"Failed to read [{typeof(T).Name}] optional property [{propertyName}] value for [{typeof(T).Name}]: {element}");
        return false;
    }

    protected bool TryGetRequiredU16Value(JsonElement element, string propertyName, out ushort value)
    {
        value = default;

        if (!TryGetRequiredProperty(element, propertyName, out var property)) return false;
        if (TryGetU16(property, out value)) return true;

        PluginLog.Warning($"Failed to read [{typeof(T).Name}] required property [{propertyName}] value for [{typeof(T).Name}]: {element}");
        return false;
    }

    private bool TryGetDict(JsonElement element, [NotNullWhen(true)] out Dictionary<string, string>? value)
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

    protected bool TryGetOptionalDictValue(JsonElement element, string propertyName, [NotNullWhen(true)] out Dictionary<string, string>? value)
    {
        if (!element.TryGetProperty(propertyName, out var property) || property.ValueKind == JsonValueKind.Null)
        {
            value = [];
            return true;
        }

        if (TryGetDict(property, out value)) return true;

        PluginLog.Warning($"Failed to read [{typeof(T).Name}] optional property [{propertyName}] values: {element}");
        return false;
    }
    protected bool TryGetRequiredDictValue(JsonElement element, string propertyName, [NotNullWhen(true)] out Dictionary<string, string>? value)
    {
        value = null;

        if (!TryGetRequiredProperty(element, propertyName, out var property)) return false;

        if (TryGetDict(property, out value)) return true;

        PluginLog.Warning($"Failed to read [{typeof(T).Name}] required property [{propertyName}] values: {element}");
        return false;
    }

    private bool TryGetArray(JsonElement element, [NotNullWhen(true)] out string[]? value)
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

    protected bool TryGetOptionalArrayValue(JsonElement element, string propertyName, [NotNullWhen(true)] out string[]? value)
    {
        if (!element.TryGetProperty(propertyName, out var property) || property.ValueKind == JsonValueKind.Null)
        {
            value = [];
            return true;
        }

        if (TryGetArray(property, out value)) return true;

        PluginLog.Warning($"Failed to read [{typeof(T).Name}] optional property [{propertyName}] values: {element}");
        return false;
    }

    protected bool TryGetRequiredArrayValue(JsonElement element, string propertyName, [NotNullWhen(true)] out string[]? value)
    {
        value = null;

        if (!TryGetRequiredProperty(element, propertyName, out var property)) return false;

        if (TryGetArray(property, out value)) return true;

        PluginLog.Warning($"Failed to read [{typeof(T).Name}] required property [{propertyName}] values: {element}");
        return false;
    }

    protected bool TryGetOptionalArrayValue(JsonElement element, string propertyName, [NotNullWhen(true)] out int[]? value)
    {
        value = null;

        if (!element.TryGetProperty(propertyName, out var property) || property.ValueKind == JsonValueKind.Null)
        {
            value = [];
            return true;
        }

        if (!IsValue(property, JsonValueKind.Array)) return false;

        var list = new List<int>();
        foreach (var item in property.EnumerateArray())
        {
            if (!IsValue(item, JsonValueKind.Number)) return false;
            if (!item.TryGetInt32(out var parsed)) return false;
            list.Add(parsed);
        }

        value = [.. list];
        return true;
    }
}
