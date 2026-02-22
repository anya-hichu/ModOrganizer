using System;
using System.IO;
using System.Text.Json;

namespace ModOrganizer.Json.Writers.Files;

public static class IFileWriterExtensions
{
    public static bool TryWriteToFile<T>(this IFileWriter<T> fileWriter, string path, T instance) where T : class
    {
        try
        {
            using var stream = new MemoryStream();
            using var jsonWriter = new Utf8JsonWriter(stream);

            if (!fileWriter.TryWrite(jsonWriter, instance)) return false;

            var data = stream.ToArray();

            File.WriteAllBytes(path, data);
            return true;
        }
        catch (Exception e)
        {
            fileWriter.PluginLog.Error($"Caught exception while writing [{typeof(T).Name}] to file ({e.Message})");
        }

        return false;
    }
}
