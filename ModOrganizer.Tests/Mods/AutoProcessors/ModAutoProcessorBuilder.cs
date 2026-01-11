using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Configs.Fakes;
using ModOrganizer.Mods;
using ModOrganizer.Mods.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Configs;
using ModOrganizer.Tests.Dalamuds.NotificationManagers;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Mods.Interops;
using ModOrganizer.Tests.Mods.Processors;

namespace ModOrganizer.Tests.Mods.AutoProcessors;

public class ModAutoProcessorBuilder : IBuilder<ModAutoProcessor>, IStubbableConfig, IStubbableNotificationManager, IStubbableModInterop, IStubbableModProcessor, IStubbablePluginLog
{
    public StubIConfig ConfigStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubINotificationManager NotificationManagerStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented }; 
    public StubIModInterop ModInteropStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIModProcessor ModProcessorStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public ModAutoProcessor Build() => new(ConfigStub, NotificationManagerStub, ModInteropStub, ModProcessorStub, PluginLogStub);
}
