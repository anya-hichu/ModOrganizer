using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Options;
using ModOrganizer.Json.Penumbra.Options.Imcs.IsDisableSubMods;
using ModOrganizer.Json.Readers.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;

namespace ModOrganizer.Tests.Json.Penumbra.Options.Imcs.IsDisableSubMods;

public class OptionImcIsDisableSubModReaderBuilder : IBuilder<OptionImcIsDisableSubModReader>, IStubbableOptionReader, IStubbablePluginLog
{
    public StubIReader<Option> OptionReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public OptionImcIsDisableSubModReader Build() => new(OptionReaderStub, PluginLogStub);
}
