using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers.Datas;

public abstract class DataReader(IPluginLog pluginLog)
{
    private JsonSerializerOptions Options { get; init; } = new() { AllowTrailingCommas = true };

    protected IPluginLog PluginLog { get; init; } = pluginLog;

    protected bool TryReadData<T>(string data, [NotNullWhen(true)] out T? instance)
    {
        instance = JsonSerializer.Deserialize<T>(data, Options);

        if (instance == null)
        {
            PluginLog.Warning($"Failed to read [{typeof(T).Name}] from data:\n\t{data}");
            return false;
        }

        return true;
    }
}
