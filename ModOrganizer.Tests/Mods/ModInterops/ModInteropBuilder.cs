using Dalamud.Plugin.Fakes;
using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Mods;
using ModOrganizer.Tests.Shared.CommandManager;
using ModOrganizer.Tests.Shared.PenumbraApis;
using ModOrganizer.Tests.Shared.PluginInterfaces;
using ModOrganizer.Tests.Shared.PluginLogs;

namespace ModOrganizer.Tests.Mods.ModInterops;

public class ModInteropBuilder : Builder<ModInterop>, IStubbableCommandManager, IStubbablePluginInterface, IStubbablePenumbraApi, IStubbablePluginLog
{
    public StubICommandManager CommandManagerStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIDalamudPluginInterface PluginInterfaceStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public override ModInterop Build() => new(CommandManagerStub, PluginInterfaceStub, PluginLogStub);
}
