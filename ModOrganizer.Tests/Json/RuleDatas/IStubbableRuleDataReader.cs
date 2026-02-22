using ModOrganizer.Json.Readers.Fakes;
using ModOrganizer.Json.RuleDatas;

namespace ModOrganizer.Tests.Json.RuleDatas;

public interface IStubbableRuleDataReader
{
    StubIReader<RuleData> RuleDataReaderStub { get; }
}
