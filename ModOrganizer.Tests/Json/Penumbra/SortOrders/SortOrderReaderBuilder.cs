using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.SortOrders;
using ModOrganizer.Json.Readers.Elements.Fakes;
using ModOrganizer.Tests.Dalamuds.PluginLogs;

namespace ModOrganizer.Tests.Json.Penumbra.SortOrders;

public class SortOrderReaderBuilder : Builder<SortOrderReader>, IStubbablePluginLog
{
    public StubIElementReader ElementReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public override SortOrderReader Build() => new(ElementReaderStub, PluginLogStub);
}
