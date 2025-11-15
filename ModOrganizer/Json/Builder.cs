using Dalamud.Plugin.Services;
using Dalamud.Utility;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;

namespace ModOrganizer.Json;

public abstract class Builder<T>(IPluginLog pluginLog) where T : class
{
    public IPluginLog PluginLog { get; set; } = pluginLog;

    public abstract bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out T? instance);

    #region Asserts

    protected bool AssertObject(JsonElement jsonElement)
    {
        if (jsonElement.ValueKind == JsonValueKind.Object) return true;

        PluginLog.Warning($"Expected root object for [{typeof(T).Name}] but got [{jsonElement.ValueKind}]:\n\t{jsonElement}");
        return false;
    }

    protected bool AssertPropertyPresent(JsonElement jsonElement, string name, out JsonElement property, bool warn = true)
    {
        if (jsonElement.TryGetProperty(name, out property)) return true;
        if (warn) PluginLog.Warning($"Expected property [{name}] for [{typeof(T).Name}] is missing:\n\t{jsonElement}");
        return false;
    }

    protected bool AssertPropertyValuePresent(JsonElement jsonElement, string name, [NotNullWhen(true)] out string? value, bool required = true)
    {
        value = null;

        if (!AssertPropertyPresent(jsonElement, name, out var property, warn: required)) return false;

        value = property.GetString();
        if (value.IsNullOrWhitespace())
        {
            PluginLog.Warning($"Property [{name}] for [{typeof(T).Name}] is null or whitespace:\n\t{property}");
            return false;
        }

        return true;
    }

    // https://github.com/xivdev/Penumbra/blob/master/schemas/structs/meta_enums.json#U8
    protected bool AssertU8PropertyValue(JsonElement jsonElement, string name, out byte value, bool required = true)
    {
        value = default;

        if (!AssertPropertyPresent(jsonElement, name, out var property, warn: required)) return false;

        if ((property.ValueKind == JsonValueKind.Number && property.TryGetByte(out value)) || (property.ValueKind == JsonValueKind.String && byte.TryParse(property.GetString(), CultureInfo.InvariantCulture, out value))) return true;

        PluginLog.Warning($"Property [{name}] for [{typeof(T).Name}] is not parsable to [{typeof(byte).Name}]:\n\t{property}");
        return false;
    }

    // https://github.com/xivdev/Penumbra/blob/master/schemas/structs/meta_enums.json#U16
    protected bool AssertU16PropertyValue(JsonElement jsonElement, string name, out ushort value, bool required = true)
    {
        value = default;

        if (!AssertPropertyPresent(jsonElement, name, out var property, warn: required)) return false;

        if ((property.ValueKind == JsonValueKind.Number && property.TryGetUInt16(out value)) || (property.ValueKind == JsonValueKind.String && ushort.TryParse(property.GetString(), CultureInfo.InvariantCulture, out value))) return true;

        PluginLog.Warning($"Property [{name}] for [{typeof(T).Name}] is not parsable to [{typeof(ushort).Name}]:\n\t{property}");
        return false;
    }

    #endregion
}
