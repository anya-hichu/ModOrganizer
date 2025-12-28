using Dalamud.Plugin.Fakes;
using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Backups;
using ModOrganizer.Configs.Fakes;
using ModOrganizer.Mods.Fakes;
using ModOrganizer.Shared.Fakes;
using ModOrganizer.Tests.Configs;
using ModOrganizer.Tests.Mods.ModInterops;
using ModOrganizer.Tests.Shared.Clock;
using ModOrganizer.Tests.Shared.PluginInterfaces;
using ModOrganizer.Tests.Shared.PluginLogs;

namespace ModOrganizer.Tests.Backups.BackupManagers;

public class BackupManagerBuilder : Builder<BackupManager>, IStubbableClock, IStubbableConfig, IStubbableModInterop, IStubbablePluginInterface, IStubbablePluginLog
{
    public StubIClock ClockStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIConfig ConfigStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIModInterop ModInteropStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIDalamudPluginInterface PluginInterfaceStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public override BackupManager Build() => new(ClockStub, ConfigStub, ModInteropStub, PluginInterfaceStub, PluginLogStub);
}
