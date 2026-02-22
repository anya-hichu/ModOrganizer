using ModOrganizer.Json.Writers;

namespace ModOrganizer.Tests.Json.RuleDatas;

public static class IStubbableRuleDataWriterExtensions
{
    public static T WithRuleDataWriterTryWriteMany<T>(this T stubbable, string? stubValue) where T : IStubbableRuleDataWriter
    {
        stubbable.RuleDataWriterStub.TryWriteManyUtf8JsonWriterIEnumerableOfT0 = (jsonWriter, instances) =>
        {
            if (stubValue == null) return false;

            jsonWriter.WriteRawValue(stubValue);
            return true;
        };

        return stubbable;
    }
}
