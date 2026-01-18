using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.LocalModDatas;
using ModOrganizer.Json.Readers.Elements.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;

namespace ModOrganizer.Tests.Json.Penumbra.LocalModDatas;

public class LocalModDataReaderBuilder : IBuilder<LocalModDataReader>, IStubbablePluginLog
{
    public StubIElementReader ElementReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public LocalModDataReader Build() => new(ElementReaderStub, PluginLogStub);
}
