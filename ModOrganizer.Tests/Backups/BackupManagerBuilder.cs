using Dalamud.Plugin.Fakes;
using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Backups;
using ModOrganizer.Configs.Fakes;
using ModOrganizer.Mods.Fakes;
using ModOrganizer.Shared.Fakes;
using ModOrganizer.Tests.Stubbables;

namespace ModOrganizer.Tests.Backups;

public class BackupManagerBuilder : IStubbablePluginLog, IStubbablePluginInterface, IStubbableConfig, IStubbableModInterop, IStubbableClock
{
    public StubIClock ClockStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIConfig ConfigStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIModInterop ModInteropStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public StubIDalamudPluginInterface PluginInterfaceStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public BackupManager Build() => new(ClockStub, ConfigStub, ModInteropStub, PluginInterfaceStub, PluginLogStub);
}
