using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers.Datas;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace ModOrganizer.Json.Readers.Clipboards;

public class ClipboardReader(IPluginLog pluginLog) : DataReader(pluginLog)
{
    public bool TryReadClipboard<T>(string data, [NotNullWhen(true)] out T? instance) where T : class
    {
        instance = null;

        try
        {
            var decodedData = Convert.FromBase64String(data);
            using MemoryStream compressedStream = new(decodedData), decompressedStream = new();
            using (var decompressor = new DeflateStream(compressedStream, CompressionMode.Decompress))
            {
                decompressor.CopyTo(decompressedStream);
            }
            var decompressedData = Encoding.UTF8.GetString(decompressedStream.ToArray());

            if (!TryReadData(decompressedData, out instance))
            {
                PluginLog.Error($"Failed to read [{typeof(T).Name}] from clipboard data");
                return false;
            }

            return true;
        }
        catch (Exception e)
        {
            PluginLog.Error($"Caught exception while reading [{typeof(T).Name}] from clipboard data ({e.Message})");
        }

        return false;
    }
}
