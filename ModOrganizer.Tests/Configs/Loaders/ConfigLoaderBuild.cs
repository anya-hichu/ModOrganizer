using Dalamud.Plugin.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Configs;
using ModOrganizer.Configs.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Configs.Defaults;
using ModOrganizer.Tests.Dalamuds.PluginInterfaces;

namespace ModOrganizer.Tests.Configs.Loaders;

public class ConfigLoaderBuild : IBuilder<ConfigLoader>, IStubbableConfigDefault, IStubbablePluginInterface
{
    public StubIConfigDefault ConfigDefaultStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIDalamudPluginInterface PluginInterfaceStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public ConfigLoader Build() => new(ConfigDefaultStub, PluginInterfaceStub);
}
