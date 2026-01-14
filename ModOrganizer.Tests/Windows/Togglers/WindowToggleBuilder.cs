using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Windows.Togglers;

namespace ModOrganizer.Tests.Windows.Togglers;

public class WindowToggleBuilder : IBuilder<WindowToggler>, IStubbablePluginLog
{
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented }  ;

    public WindowToggler Build() => new(PluginLogStub);
}
