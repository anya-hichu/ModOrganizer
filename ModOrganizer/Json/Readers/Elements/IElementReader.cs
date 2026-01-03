using System.Text.Json;

namespace ModOrganizer.Json.Readers.Elements;

public interface IElementReader
{
    bool TryReadFromData(string data, out JsonElement instance);
    bool TryReadFromFile(string path, out JsonElement instance);
}
