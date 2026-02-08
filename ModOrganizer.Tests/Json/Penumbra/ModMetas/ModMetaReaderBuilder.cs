using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.ModMetas;
using ModOrganizer.Json.Readers.Elements.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Readers.Elements;

namespace ModOrganizer.Tests.Json.Penumbra.ModMetas;

public class ModMetaReaderBuilder : IBuilder<ModMetaReader>, IStubbableElementReader, IStubbablePluginLog
{
    public StubIElementReader ElementReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public ModMetaReader Build() => new(ElementReaderStub, PluginLogStub);
}
