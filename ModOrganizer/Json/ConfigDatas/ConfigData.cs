using ModOrganizer.Json.RuleDatas;

namespace ModOrganizer.Json.ConfigDatas;

// No schema
public class ConfigData : Data
{
    // unused currently
    public int? Version { get; init; }
    public RuleData[]? Rules { get; init; }
}
