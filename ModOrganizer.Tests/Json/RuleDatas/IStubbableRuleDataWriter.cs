using ModOrganizer.Json.RuleDatas;
using ModOrganizer.Json.Writers.Fakes;

namespace ModOrganizer.Tests.Json.RuleDatas;

public interface IStubbableRuleDataWriter
{
    StubIWriter<RuleData> RuleDataWriterStub { get; }
}
