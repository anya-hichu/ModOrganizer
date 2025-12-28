using ModOrganizer.Rules.Fakes;

namespace ModOrganizer.Tests.Rules.RuleEvaluators;

public interface IStubbableRuleEvaluator
{
    StubIRuleEvaluator RuleEvaluatorStub { get; }
}
