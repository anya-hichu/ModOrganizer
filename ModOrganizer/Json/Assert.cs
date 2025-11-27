using Dalamud.Plugin.Services;
using Dalamud.Utility;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;

namespace ModOrganizer.Json;

public class Assert(IPluginLog pluginLog)
{
    private IPluginLog PluginLog { get; init; } = pluginLog;

    public bool IsObject(JsonElement jsonElement)
    {
        if (jsonElement.ValueKind == JsonValueKind.Object) return true;

        PluginLog.Warning($"Expected root object but got [{jsonElement.ValueKind}]:\n\t{jsonElement}");
        return false;
    }

    public bool IsPropertyPresent(JsonElement jsonElement, string name, out JsonElement property, bool warn = true)
    {
        if (jsonElement.TryGetProperty(name, out property)) return true;
        if (warn) PluginLog.Warning($"Expected property [{name}] is missing:\n\t{jsonElement}");
        return false;
    }

    public bool IsPropertyValuePresent(JsonElement jsonElement, string name, [NotNullWhen(true)] out string? value, bool required = true)
    {
        value = null;

        if (!IsPropertyPresent(jsonElement, name, out var property, warn: required)) return false;

        value = property.GetString();
        if (value.IsNullOrEmpty())
        {
            PluginLog.Warning($"Property [{name}] is null or empty:\n\t{property}");
            return false;
        }

        return true;
    }

    // https://github.com/xivdev/Penumbra/blob/master/schemas/structs/meta_enums.json#U8
    public bool IsU8PropertyValue(JsonElement jsonElement, string name, out byte value, bool required = true)
    {
        value = default;

        if (!IsPropertyPresent(jsonElement, name, out var property, warn: required)) return false;

        if ((property.ValueKind == JsonValueKind.Number && property.TryGetByte(out value)) || (property.ValueKind == JsonValueKind.String && byte.TryParse(property.GetString(), CultureInfo.InvariantCulture, out value))) return true;

        PluginLog.Warning($"Property [{name}] is not parsable as [{typeof(byte).Name}]:\n\t{property}");
        return false;
    }

    // https://github.com/xivdev/Penumbra/blob/master/schemas/structs/meta_enums.json#U16
    public bool IsU16PropertyValue(JsonElement jsonElement, string name, out ushort value, bool required = true)
    {
        value = default;

        if (!IsPropertyPresent(jsonElement, name, out var property, warn: required)) return false;

        if ((property.ValueKind == JsonValueKind.Number && property.TryGetUInt16(out value)) || (property.ValueKind == JsonValueKind.String && ushort.TryParse(property.GetString(), CultureInfo.InvariantCulture, out value))) return true;

        PluginLog.Warning($"Property [{name}] is not parsable as [{typeof(ushort).Name}]:\n\t{property}");
        return false;
    }
}
