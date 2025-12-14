using System;

namespace ModOrganizer.Rules;

[Serializable]
public class Rule : IComparable<Rule>
{
    public bool Enabled { get; set; } = false;
    public string Name { get; set; } = string.Empty;
    public int Priority { get; set; } = 0;
    public string MatchExpression { get; set; } = string.Empty;
    public string PathTemplate { get; set; } = string.Empty;

    public int CompareTo(Rule? other) => Priority.CompareTo(other?.Priority);
}
