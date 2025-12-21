namespace ModOrganizer.Json.RuleDatas;

public class RuleData
{
    public bool? Enabled { get; set; }
    public required string Path { get; set; }
    public int? Priority { get; set; }
    public string? MatchExpression { get; set; }
    public string? PathTemplate { get; set; }
}
