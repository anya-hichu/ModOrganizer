using Dalamud.Configuration;
using ModOrganizer.Backups;
using ModOrganizer.Rules;
using System;
using System.Collections.Generic;

namespace ModOrganizer;

[Serializable]
public class Config : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool AutoProcessEnabled { get; set; } = false;
    public uint AutoProcessWaitMs { get; set; } = 1000;

    public List<Rule> Rules { get; set; } = [];

    public bool AutoBackupEnabled { get; set; } = true;
    public uint AutoBackupWaitMs { get; set; } = 1000;
    public List<Backup> Backups { get; set; } = [];
}
