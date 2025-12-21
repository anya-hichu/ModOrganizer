using Dalamud.Configuration;
using ModOrganizer.Backups;
using ModOrganizer.Rules;
using System.Collections.Generic;

namespace ModOrganizer.Configs;

public interface IConfig : IPluginConfiguration
{
    HashSet<Rule> Rules { get; set; }

    bool AutoProcessEnabled { get; set; }
    uint AutoProcessDelayMs { get; set; }

    HashSet<Backup> Backups { get; set; }

    bool AutoBackupEnabled { get; set; }
    uint AutoBackupLimit { get; set; }
}
