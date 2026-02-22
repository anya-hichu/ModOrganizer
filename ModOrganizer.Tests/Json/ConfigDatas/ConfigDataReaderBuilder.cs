using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Json.ConfigDatas;
using ModOrganizer.Json.Readers.Elements.Fakes;
using ModOrganizer.Json.Readers.Fakes;
using ModOrganizer.Json.RuleDatas;
using ModOrganizer.Shared;
using ModOrganizer.Tests.Dalamuds.PluginLogs;
using ModOrganizer.Tests.Json.Readers.Elements;
using ModOrganizer.Tests.Json.RuleDatas;

namespace ModOrganizer.Tests.Json.ConfigDatas;

public class ConfigDataReaderBuilder : IBuilder<ConfigDataReader>, IStubbableElementReader, IStubbableRuleDataReader, IStubbablePluginLog
{
    public StubIElementReader ElementReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIReader<RuleData> RuleDataReaderStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public ConfigDataReader Build() => new(ElementReaderStub, RuleDataReaderStub, PluginLogStub);
}
