using Dalamud.Plugin.Fakes;
using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.DefaultMods.Fakes;
using ModOrganizer.Json.Penumbra.Groups.Fakes;
using ModOrganizer.Json.Penumbra.LocalModDatas.Fakes;
using ModOrganizer.Json.Penumbra.ModMetas.Fakes;
using ModOrganizer.Json.Penumbra.SortOrders.Fakes;
using ModOrganizer.Mods;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.CommandManagers;
using ModOrganizer.Tests.Dalamuds.PenumbraApis;
using ModOrganizer.Tests.Dalamuds.PluginInterfaces;
using ModOrganizer.Tests.Dalamuds.PluginLogs;

namespace ModOrganizer.Tests.Mods.ModInterops;

public class ModInteropBuilder : IBuilder<ModInterop>, IStubbableCommandManager, IStubbablePluginInterface, IStubbablePenumbraApi, IStubbablePluginLog
{
    public StubICommandManager CommandManagerStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIDalamudPluginInterface PluginInterfaceStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public StubIDefaultModReader DefaultModReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIGroupGenericReader GroupGenericReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubILocalModDataReader LocalModDataReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIModMetaReader ModMetaReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubISortOrderReader SortOrderFileReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public ModInterop Build() => new(CommandManagerStub, DefaultModReaderStub, GroupGenericReaderStub, LocalModDataReaderStub, ModMetaReaderStub, PluginInterfaceStub, PluginLogStub, SortOrderFileReaderStub);
}
