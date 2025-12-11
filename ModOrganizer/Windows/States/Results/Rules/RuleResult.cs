using ModOrganizer.Windows.States.Results.Showables;

namespace ModOrganizer.Windows.States.Results.Rules;

public abstract class RuleResult : Result, IShowableRuleResult
{
    public required string CurrentPath { get; init; }

    public abstract bool IsShowed(IShowableRuleResultState state);
}
