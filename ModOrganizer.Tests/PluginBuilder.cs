using Dalamud.Plugin.Fakes;
using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Tests.Dalamuds.CommandManagers;
using ModOrganizer.Tests.Dalamuds.PenumbraApis;
using ModOrganizer.Tests.Dalamuds.PluginInterfaces;
using ModOrganizer.Tests.Dalamuds.PluginLogs;

namespace ModOrganizer.Tests;

public class PluginBuilder : Builder<Plugin>, IStubbableCommandManager, IStubbablePluginLog, IStubbablePluginInterface, IStubbablePenumbraApi
{
    public StubIDalamudPluginInterface PluginInterfaceStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubICommandManager CommandManagerStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public override Plugin Build()
    {
        Plugin.PluginLog = PluginLogStub;
        Plugin.PluginInterface = PluginInterfaceStub;
        Plugin.CommandManager = CommandManagerStub;

        return new();
    }
}
