using Dalamud.Plugin.Services;
using ModOrganizer.Json.ConfigDatas;
using ModOrganizer.Shared;
using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Configs;

public class ConfigConverter(IPluginLog pluginLog) : Converter<Config, ConfigData>(pluginLog)
{
    public override bool TryConvert(Config input, [NotNullWhen(true)] out ConfigData? output)
    {
        throw new System.NotImplementedException();
    }
}
