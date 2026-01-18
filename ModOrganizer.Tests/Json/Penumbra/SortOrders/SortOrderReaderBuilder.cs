using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.SortOrders;
using ModOrganizer.Json.Readers.Elements.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Readers.Elements;

namespace ModOrganizer.Tests.Json.Penumbra.SortOrders;

public class SortOrderReaderBuilder : IBuilder<SortOrderReader>, IStubbableElementReader, IStubbablePluginLog
{
    public StubIElementReader ElementReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public SortOrderReader Build() => new(ElementReaderStub, PluginLogStub);
}
