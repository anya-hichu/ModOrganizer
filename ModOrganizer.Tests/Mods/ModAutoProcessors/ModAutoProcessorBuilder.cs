using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Configs.Fakes;
using ModOrganizer.Mods;
using ModOrganizer.Mods.Fakes;
using ModOrganizer.Tests.Configs;
using ModOrganizer.Tests.Mods.ModInterops;
using ModOrganizer.Tests.Mods.ModProcessors;
using ModOrganizer.Tests.Shared.NotificationManager;
using ModOrganizer.Tests.Shared.PluginLogs;

namespace ModOrganizer.Tests.Mods.ModAutoProcessors;

public class ModAutoProcessorBuilder : Builder<ModAutoProcessor>, IStubbableConfig, IStubbableNotificationManager, IStubbableModInterop, IStubbableModProcessor, IStubbablePluginLog
{
    public StubIConfig ConfigStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubINotificationManager NotificationManagerStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented }; 
    public StubIModInterop ModInteropStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIModProcessor ModProcessorStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public override ModAutoProcessor Build() => new(ConfigStub, NotificationManagerStub, ModInteropStub, ModProcessorStub, PluginLogStub);
}
