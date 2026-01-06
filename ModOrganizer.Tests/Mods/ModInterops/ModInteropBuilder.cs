using Dalamud.Plugin.Fakes;
using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.DefaultMods;
using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Json.Penumbra.LocalModDatas;
using ModOrganizer.Json.Penumbra.ModMetas;
using ModOrganizer.Json.Penumbra.SortOrders;
using ModOrganizer.Json.Readers.Files.Fakes;
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

    public StubIFileReader<DefaultMod> DefaultModFileReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIFileReader<Group> GroupFileReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIFileReader<LocalModData> LocalModDataFileReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIFileReader<ModMeta> ModMetaFileReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIFileReader<SortOrder> SortOrderFileReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public ModInterop Build() => new(CommandManagerStub, DefaultModFileReaderStub, GroupFileReaderStub, LocalModDataFileReaderStub, ModMetaFileReaderStub, PluginInterfaceStub, PluginLogStub, SortOrderFileReaderStub);
}
