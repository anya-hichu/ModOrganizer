using Dalamud.Plugin.Fakes;
using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Backups;
using ModOrganizer.Configs.Fakes;
using ModOrganizer.Mods.Fakes;
using ModOrganizer.Shared.Fakes;

namespace ModOrganizer.Tests.Backups;

public class BackupManagerBuilder
{
    public StubIClock ClockStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIConfig ConfigStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIModInterop ModInteropStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public StubIDalamudPluginInterface PluginInterfaceStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public StubObserver PluginLogObserver { get; init; } = new();
    public StubIPluginLog PluginLogStub { get; init; }

    public BackupManagerBuilder()
    {
        PluginLogStub = new()
        {
            InstanceBehavior = StubBehaviors.NotImplemented,
            InstanceObserver = PluginLogObserver
        };
    }

    public BackupManagerBuilder WithConfigBackups(HashSet<Backup> backups)
    {
        ConfigStub.BackupsGet = () => backups;
        return this;
    }

    public BackupManagerBuilder WithModInteropSortOrderPath(string path, bool exists = false)
    {
        if (exists && !File.Exists(path)) File.WriteAllText(path, string.Empty);
        ModInteropStub.GetSortOrderPath = () => path;
        return this;
    }

    public BackupManagerBuilder WithPluginInterfaceConfigDirectory(DirectoryInfo configDirectory)
    {
        PluginInterfaceStub.ConfigDirectoryGet = () => configDirectory;
        return this;
    }

    public BackupManagerBuilder WithPluginInterfaceSaveConfigNoop()
    {
        PluginInterfaceStub.SavePluginConfigIPluginConfiguration = config => { };
        return this;
    }

    public BackupManagerBuilder WithClockNewUtc(DateTimeOffset offset)
    {
        ClockStub.GetNowUtc = () => offset;
        return this;
    }

    public BackupManagerBuilder WithConfigAutoBackupLimit(ushort value)
    {
        ConfigStub.AutoBackupLimitGet = () => value;
        return this;
    }

    public BackupManagerBuilder WithPluginLogDefaults()
    {
        PluginLogStub.BehaveAsDefaultValue();
        return this;
    }


    public BackupManager Build() => new(ClockStub, ConfigStub, ModInteropStub, PluginInterfaceStub, PluginLogStub);
}
