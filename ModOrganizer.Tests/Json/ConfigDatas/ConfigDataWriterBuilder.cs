using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.ConfigDatas;
using ModOrganizer.Json.RuleDatas;
using ModOrganizer.Json.Writers.Fakes;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.RuleDatas;

namespace ModOrganizer.Tests.Json.ConfigDatas;

public class ConfigDataWriterBuilder : IBuilder<ConfigDataWriter>, IStubbableRuleDataWriter, IStubbablePluginLog
{
    public StubIWriter<RuleData> RuleDataWriterStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public ConfigDataWriter Build() => new(RuleDataWriterStub, PluginLogStub);
}
