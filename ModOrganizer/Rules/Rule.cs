using System;

namespace ModOrganizer.Rules;

[Serializable]
public class Rule : IComparable<Rule>, IEquatable<Rule>
{
    public bool Enabled { get; set; } = false;
    public string Path { get; set; } = string.Empty;
    public int Priority { get; set; } = 0;
    public string MatchExpression { get; set; } = string.Empty;
    public string PathTemplate { get; set; } = string.Empty;

    public int CompareTo(Rule? other) => Priority.CompareTo(other?.Priority);

    public override bool Equals(object? obj) => Equals(obj as Rule);
    public bool Equals(Rule? other) => other != null && GetHashCode() == other.GetHashCode();
    public override int GetHashCode() => Path.GetHashCode();
}
