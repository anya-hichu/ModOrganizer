using Dalamud.Plugin.Services;
using ModOrganizer.Json.Readers.Datas;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace ModOrganizer.Json.Readers.Files;

public class FileReader(IPluginLog pluginLog) : DataReader(pluginLog), IFileReader
{
    public bool TryReadFile<T>(string path, [NotNullWhen(true)] out T? instance)
    {
        instance = default;

        try
        {
            if (TryReadData(File.ReadAllText(path), out instance)) return true;

            PluginLog.Error($"Failed to read [{typeof(T).Name}] from json file [{path}]");
        }
        catch (Exception e)
        {
            PluginLog.Error($"Caught exception while reading [{typeof(T).Name}] from json file [{path}] ({e.Message})");
        }

        return false;
    }
}
