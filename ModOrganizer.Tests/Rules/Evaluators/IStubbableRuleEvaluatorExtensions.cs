namespace ModOrganizer.Tests.Rules.Evaluators;

public static class IStubbableRuleEvaluatorExtensions
{
    public static T WithRuleEvaluatorTryEvaluateMany<T>(this T stubbable, string? stubValue) where T : IStubbableRuleEvaluator
    {
        stubbable.RuleEvaluatorStub.TryEvaluateManyIEnumerableOfRuleModInfoStringOut = (rules, modInfo, out path) =>
        {
            path = stubValue;
            return stubValue != null;
        };

        return stubbable;
    }
}
