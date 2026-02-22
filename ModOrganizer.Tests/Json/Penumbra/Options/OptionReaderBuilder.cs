using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Options;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;

namespace ModOrganizer.Tests.Json.Penumbra.Options;

public class OptionReaderBuilder : IBuilder<OptionReader>, IStubbablePluginLog
{
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public OptionReader Build() => new(PluginLogStub);
}
