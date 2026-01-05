using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Clipboards;

public static class IClipboardReaderExtensions
{
    public static bool TryReadFromClipboard<T>(this IClipboardReader<T> clipboardReader, string data, [NotNullWhen(true)] out T? instance) where T : class
    {
        instance = null;

        try
        {
            var decodedData = Convert.FromBase64String(data);

            using MemoryStream compressedStream = new(decodedData), decompressedStream = new();
            using (var decompressor = new DeflateStream(compressedStream, CompressionMode.Decompress)) decompressor.CopyTo(decompressedStream);

            var decompressedData = Encoding.UTF8.GetString(decompressedStream.ToArray());

            if (!clipboardReader.ElementReader.TryReadFromData(decompressedData, out var element)) return false;

            if (clipboardReader.TryRead(element, out instance)) return true;

            clipboardReader.PluginLog.Error($"Failed to read [{typeof(T).Name}] from clipboard data: {decompressedData}");
        }
        catch (Exception e)
        {
            clipboardReader.PluginLog.Error($"Caught exception while reading [{typeof(T).Name}] from clipboard data ({e.Message}): {data}");
        }

        return false;
    }
}
