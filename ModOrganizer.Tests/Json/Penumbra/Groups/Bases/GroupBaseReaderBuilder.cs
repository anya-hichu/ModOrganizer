using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.Penumbra.Groups.Bases;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;

namespace ModOrganizer.Tests.Json.Penumbra.Groups.Bases;

public class GroupBaseReaderBuilder : IBuilder<GroupBaseReader>, IStubbablePluginLog
{
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public GroupBaseReader Build() => new(PluginLogStub);
}
