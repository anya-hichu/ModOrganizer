using ModOrganizer.Rules.Fakes;

namespace ModOrganizer.Tests.Rules.Evaluators;

public interface IStubbableRuleEvaluator
{
    StubIRuleEvaluator RuleEvaluatorStub { get; }
}
