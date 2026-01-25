using ModOrganizer.Json.RuleDatas;

namespace ModOrganizer.Json.ConfigDatas;

// No schema
public class ConfigData : Data
{
    public required int Version { get; init; }
    public required RuleData[] Rules { get; init; }
}
