using Dalamud.Plugin.Fakes;
using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Configs.Fakes;
using ModOrganizer.Mods.Fakes;
using ModOrganizer.Shared.Fakes;

namespace ModOrganizer.Tests.Backups;

public class BackupManagerFakes
{
    private Random Random { get; init; } = new();

    public StubIClock Clock { get; init; }
    public StubIConfig Config { get; init; }
    public StubIModInterop ModInterop { get; init; }

    public DirectoryInfo ConfigDirectory { get; init; }
    public StubIDalamudPluginInterface PluginInterface { get; init; }

    public StubObserver PluginLogObserver { get; init; }
    public StubIPluginLog PluginLog { get; init; }

    public BackupManagerFakes(string tempDirectory)
    {
        Clock = new StubIClock()
        {
            // Non-deterministic clock
            GetNowUtc = () => DateTimeOffset.UtcNow.AddHours(Random.Next(short.MinValue, 0))
        };

        Config = new StubIConfig() 
        { 
            AutoBackupLimitGet = () => ushort.MaxValue, 
            InstanceBehavior = StubBehaviors.NotImplemented 
        };

        ModInterop = new StubIModInterop() { InstanceBehavior = StubBehaviors.NotImplemented };

        ConfigDirectory = Directory.CreateDirectory(Path.Combine(tempDirectory, nameof(ModOrganizer)));
        PluginInterface = new StubIDalamudPluginInterface()
        {
            ConfigDirectoryGet = () => ConfigDirectory,
            SavePluginConfigIPluginConfiguration = config => { },
            InstanceBehavior = StubBehaviors.NotImplemented
        };

        PluginLogObserver = new();

        PluginLog = new StubIPluginLog() 
        { 
            InstanceBehavior = StubBehaviors.DefaultValue, 
            InstanceObserver = PluginLogObserver 
        };
    }
}
