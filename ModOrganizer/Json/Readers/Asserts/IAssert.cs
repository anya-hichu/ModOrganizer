using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Asserts;

public interface IAssert
{
    bool IsValue(JsonElement element, JsonValueKind kind);
    bool IsPropertyPresent(JsonElement element, string name, out JsonElement property, bool warn = true);
    bool IsValuePresent(JsonElement element, string propertyName, [NotNullWhen(true)] out string? value, bool required = true);
    bool IsOptionalValue(JsonElement element, string propertyName, out string? value);
    bool IsU8Value(JsonElement element, string propertyName, out byte value, bool required = true);
    bool IsU16Value(JsonElement element, string propertyName, out ushort value, bool required = true);
    bool IsStringDict(JsonElement element, [NotNullWhen(true)] out Dictionary<string, string>? value);
    bool IsStringArray(JsonElement element, [NotNullWhen(true)] out string[]? value);
    bool IsIntArray(JsonElement element, [NotNullWhen(true)] out int[]? value);
}
