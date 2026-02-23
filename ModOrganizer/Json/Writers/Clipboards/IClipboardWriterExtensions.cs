using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Text.Json;

namespace ModOrganizer.Json.Writers.Clipboards;

public static class IClipboardWriterExtensions
{
    public static bool TryWriteToClipboard<T>(this IClipboardWriter<T> clipboardWriter, T instance, [NotNullWhen(true)] out string? data) where T : class
    {
        data = null;

        try
        {
            using MemoryStream compressedStream = new(), stream = new();

            using (var jsonWriter = new Utf8JsonWriter(stream))
            {
                if (!clipboardWriter.TryWrite(jsonWriter, instance)) return false;
            }
            
            stream.Position = 0;
            using (var compressor = new DeflateStream(compressedStream, CompressionMode.Compress)) stream.CopyTo(compressor);

            var bytes = compressedStream.ToArray();
            data = Convert.ToBase64String(bytes);

            return true;
        }
        catch (Exception e)
        {
            clipboardWriter.PluginLog.Error($"Caught exception while writing [{typeof(T).Name}] to clipboard data ({e.Message})");
        }

        return false;
    }
}
