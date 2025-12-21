using ModOrganizer.Backups;
using ModOrganizer.Rules;
using System;
using System.Collections.Generic;

namespace ModOrganizer.Configs;

[Serializable]
public class Config : IConfig
{
    public int Version { get; set; } = 0;

    public HashSet<Rule> Rules { get; set; } = [];

    public bool AutoProcessEnabled { get; set; } = false;
    public uint AutoProcessDelayMs { get; set; } = 1000;

    public HashSet<Backup> Backups { get; set; } = [];

    public bool AutoBackupEnabled { get; set; } = true;
    public uint AutoBackupLimit { get; set; } = 10;
}
