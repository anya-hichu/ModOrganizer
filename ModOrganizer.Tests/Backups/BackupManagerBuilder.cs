using Dalamud.Plugin.Fakes;
using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Backups;
using ModOrganizer.Configs.Fakes;
using ModOrganizer.Json.Penumbra.SortOrders.Fakes;
using ModOrganizer.Mods.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Configs;
using ModOrganizer.Tests.Dalamuds.PluginInterfaces;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Mods.Interops;

namespace ModOrganizer.Tests.Backups;

public class BackupManagerBuilder : IBuilder<BackupManager>, IStubbableConfig, IStubbableModInterop, IStubbablePluginInterface, IStubbablePluginLog
{
    public StubIConfig ConfigStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIModInterop ModInteropStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIDalamudPluginInterface PluginInterfaceStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubISortOrderReader SortOrderFileReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public BackupManager Build() => new(ConfigStub, ModInteropStub, PluginInterfaceStub, PluginLogStub, SortOrderFileReaderStub);
}
