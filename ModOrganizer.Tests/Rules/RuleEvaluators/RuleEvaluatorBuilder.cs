using Dalamud.Plugin.Services.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Rules;
using ModOrganizer.Tests.Dalamuds.PluginLogs;

namespace ModOrganizer.Tests.Rules.RuleEvaluators;

public class RuleEvaluatorBuilder : Builder<RuleEvaluator>, IStubbablePluginLog
{
    public StubIPluginLog PluginLogStub { get; init; } = new() { InstanceBehavior = StubBehaviors.NotImplemented };

    public override RuleEvaluator Build() => new(PluginLogStub);
}
