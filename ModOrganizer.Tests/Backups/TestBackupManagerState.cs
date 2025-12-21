using Dalamud.Plugin.Fakes;
using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Backups;
using ModOrganizer.Configs.Fakes;
using ModOrganizer.Mods.Fakes;

namespace ModOrganizer.Tests.Backups;

public class TestBackupManagerState : IDisposable
{
    public StubIConfig ConfigStub { get; init; }
    public StubIModInterop ModInteropStub { get; init; }

    public DirectoryInfo ConfigDirectory { get; init; }
    public StubIDalamudPluginInterface PluginInterfaceStub { get; init; }

    public StubObserver PluginLogObserver { get; init; }
    public StubIPluginLog PluginLogStub { get; init; }

    public BackupManager BackupManager { get; init; }

    public TestBackupManagerState()
    {
        ConfigStub = new StubIConfig() { InstanceBehavior = StubBehaviors.NotImplemented };
        ModInteropStub = new StubIModInterop() { InstanceBehavior = StubBehaviors.NotImplemented };

        ConfigDirectory = Directory.CreateDirectory(nameof(ModOrganizer));
        PluginInterfaceStub = new StubIDalamudPluginInterface()
        {
            ConfigDirectoryGet = () => ConfigDirectory,
            InstanceBehavior = StubBehaviors.DefaultValue
        };

        PluginLogObserver = new();
        PluginLogStub = new StubIPluginLog() { InstanceBehavior = StubBehaviors.DefaultValue, InstanceObserver = PluginLogObserver };

        BackupManager = new(ConfigStub, ModInteropStub, PluginInterfaceStub, PluginLogStub);
    }

    public void Dispose() => Directory.Delete(ConfigDirectory.FullName, true);
}
