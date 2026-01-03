using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Files;

public static class IReadableFromFileExtensions
{
    public static bool TryReadFromFile<T>(this IReadableFromFile<T> readableFile, string path, [NotNullWhen(true)] out T? instance) where T : class
    {
        instance = null;

        if (!readableFile.FileReader.TryReadFile<JsonElement>(path, out var jsonElement))
        {
            readableFile.PluginLog.Warning($"Failed to read [{nameof(JsonElement)}] from json file [{path}]");
            return false;
        }

        if (!readableFile.TryRead(jsonElement, out instance))
        {
            readableFile.PluginLog.Debug($"Failed to read instance [{typeof(T).Name}] from json file [{path}]");
            return false;
        }

        return true;
    }
}
