using System;
using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Json.Readers.Files;

public static class IFileReaderExtensions
{
    public static bool TryReadFromFile<T>(this IFileReader<T> fileReader, string path, [NotNullWhen(true)] out T? instance) where T : class
    {
        instance = null;

        try
        {
            if (!fileReader.ElementReader.TryReadFromFile(path, out var element)) return false;

            if (fileReader.TryRead(element, out instance)) return true;

            fileReader.PluginLog.Warning($"Failed to read instance [{typeof(T).Name}] from json file [{path}]");
            return false;
        }
        catch (Exception e)
        {
            fileReader.PluginLog.Error($"Caught exception while reading [{typeof(T).Name}] from json file [{path}] ({e.Message})");
        }

        return false;
    }
}
