namespace ModOrganizer.Tests.Rules.RuleEvaluators;

public static class IStubbableRuleEvaluatorExtensions
{
    public static T WithRuleEvaluatorTryEvaluateMany<T>(this T stubbable, string? value) where T : IStubbableRuleEvaluator
    {
        stubbable.RuleEvaluatorStub.TryEvaluateManyIEnumerableOfRuleModInfoStringOut = (rules, modInfo, out path) =>
        {
            path = value;
            return value != null;
        };

        return stubbable;
    }
}
