using ModOrganizer.Json.RuleDatas;

namespace ModOrganizer.Tests.Json.RuleDatas;

public static class IStubbableRuleDataReaderExtensions
{
    public static T WithRuleDataReaderTryReadMany<T>(this T stubbable, RuleData[]? stubValue) where T : IStubbableRuleDataReader
    {
        stubbable.RuleDataReaderStub.TryReadManyJsonElementT0ArrayOut = (element, out instances) =>
        {
            instances = stubValue!;
            return stubValue != null;
        };

        return stubbable;
    }
}
