using Dalamud.Plugin.Fakes;
using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Backups;
using ModOrganizer.Configs.Fakes;
using ModOrganizer.Mods.Fakes;
using ModOrganizer.Tests.Configs;
using ModOrganizer.Tests.Mods.ModInterops;
using ModOrganizer.Tests.Dalamuds.PluginInterfaces;
using ModOrganizer.Tests.Dalamuds.PluginLogs;

namespace ModOrganizer.Tests.Backups.BackupManagers;

public class BackupManagerBuilder : Builder<BackupManager>, IStubbableConfig, IStubbableModInterop, IStubbablePluginInterface, IStubbablePluginLog
{
    public StubIConfig ConfigStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIModInterop ModInteropStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIDalamudPluginInterface PluginInterfaceStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public override BackupManager Build() => new(ConfigStub, ModInteropStub, PluginInterfaceStub, PluginLogStub);
}
