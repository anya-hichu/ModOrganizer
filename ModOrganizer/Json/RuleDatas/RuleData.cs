namespace ModOrganizer.Json.RuleDatas;

public class RuleData
{
    public required bool Enabled { get; set; }
    public required string Path { get; set; }
    public required int Priority { get; set; }
    public required string MatchExpression { get; set; }
    public required string PathTemplate { get; set; }
}
