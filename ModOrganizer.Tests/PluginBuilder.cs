using Dalamud.Plugin.Fakes;
using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Providers;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.CommandManagers;
using ModOrganizer.Tests.Dalamuds.PenumbraApis;
using ModOrganizer.Tests.Dalamuds.PluginInterfaces;
using ModOrganizer.Tests.Dalamuds.PluginLogs;

namespace ModOrganizer.Tests;

public class PluginBuilder : IBuilder<Plugin>, IStubbableCommandManager, IStubbablePluginLog, IStubbablePluginInterface, IStubbablePenumbraApi
{
    public StubICommandManager CommandManagerStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubINotificationManager NotificationManagerStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIDalamudPluginInterface PluginInterfaceStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public Plugin Build()
    {
        PluginInterfaceStub.CreateOf1ObjectArray(_ =>
        {
            var provider = new RootProvider();

            provider.CommandManager = CommandManagerStub;
            provider.NotificationManager = NotificationManagerStub;
            provider.PluginInterface = PluginInterfaceStub;
            provider.PluginLog = PluginLogStub;

            return provider;
        });

        return new(PluginInterfaceStub);
    }
}
