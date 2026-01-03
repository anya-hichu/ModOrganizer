using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Elements;

public interface IElementReader
{
    bool TryReadFromData(string data, [NotNullWhen(true)] out JsonElement instance);
}
