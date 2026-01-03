using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Files;

public static class IFileReaderExtensions
{
    public static bool TryReadFromFile<T>(this IFileReader<T> fileReader, string path, [NotNullWhen(true)] out T? instance) where T : class
    {
        instance = null;

        if (!fileReader.ElementReader.TryReadFromData(File.ReadAllText(path), out var jsonElement))
        {
            fileReader.PluginLog.Warning($"Failed to read [{nameof(JsonElement)}] from json file [{path}]");
            return false;
        }

        if (!fileReader.TryRead(jsonElement, out instance))
        {
            fileReader.PluginLog.Debug($"Failed to read instance [{typeof(T).Name}] from json file [{path}]");
            return false;
        }

        return true;
    }
}
