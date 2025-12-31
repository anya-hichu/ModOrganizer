using Dalamud.Plugin.Fakes;
using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Mods;
using ModOrganizer.Tests.Dalamuds.CommandManagers;
using ModOrganizer.Tests.Dalamuds.PenumbraApis;
using ModOrganizer.Tests.Dalamuds.PluginInterfaces;
using ModOrganizer.Tests.Dalamuds.PluginLogs;

namespace ModOrganizer.Tests.Mods.ModInterops;

public class ModInteropBuilder : Builder<ModInterop>, IStubbableCommandManager, IStubbablePluginInterface, IStubbablePenumbraApi, IStubbablePluginLog
{
    public StubICommandManager CommandManagerStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIDalamudPluginInterface PluginInterfaceStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public override ModInterop Build() => new(CommandManagerStub, PluginInterfaceStub, PluginLogStub);
}
