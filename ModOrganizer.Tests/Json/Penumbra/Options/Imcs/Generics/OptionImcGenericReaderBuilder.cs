using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Options.Imcs.AttributeMasks.Fakes;
using ModOrganizer.Json.Penumbra.Options.Imcs.Generics;
using ModOrganizer.Json.Penumbra.Options.Imcs.IsDisableSubMods.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Penumbra.Options.Imcs.AttributeMasks;
using ModOrganizer.Tests.Json.Penumbra.Options.Imcs.IsDisableSubMods;

namespace ModOrganizer.Tests.Json.Penumbra.Options.Imcs.Generics;

public class OptionImcGenericReaderBuilder : IBuilder<OptionImcGenericReader>, IStubbableOptionImcAttributeMaskReader, IStubbableOptionImcIsDisableSubModReader, IStubbablePluginLog
{
    public StubIOptionImcAttributeMaskReader OptionImcAttributeMaskReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIOptionImcIsDisableSubModReader OptionImcIsDisableSubModReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public OptionImcGenericReader Build() => new(OptionImcAttributeMaskReaderStub, OptionImcIsDisableSubModReaderStub, PluginLogStub);
}
