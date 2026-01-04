using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Asserts.Fakes;
using ModOrganizer.Json.Penumbra.SortOrders;
using ModOrganizer.Json.Readers.Elements.Fakes;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Asserts;
using ModOrganizer.Tests.Json.Readers.Elements;

namespace ModOrganizer.Tests.Json.Penumbra.SortOrders;

public class SortOrderReaderBuilder : Builder<SortOrderReader>, IStubbableAssert, IStubbableElementReader, IStubbablePluginLog
{
    public StubIAssert AssertStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIElementReader ElementReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public override SortOrderReader Build() => new(AssertStub, ElementReaderStub, PluginLogStub);
}
