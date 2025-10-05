using Dalamud.Configuration;
using ModOrganizer.Rules;
using System;
using System.Collections.Generic;

namespace ModOrganizer;

[Serializable]
public class Config : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool AutoProcessEnabled { get; set; } = true;
    public uint AutoProcessDelayMs { get; set; } = 1000;

    public List<Rule> Rules { get; set; } = [];

    public void Save()
    {
        Plugin.PluginInterface.SavePluginConfig(this);
    }
}
