using ModOrganizer.Shared;
using ModOrganizer.Windows.Results.Showables;

namespace ModOrganizer.Windows.Results.Rules;

public abstract class RuleResult : Result, IShowableResult<IShowableRuleResultState>
{
    public required string Path { get; init; }

    public virtual bool IsShowed(IShowableRuleResultState state) => base.IsShowed(state) && TokenMatcher.Matches(state.PathFilter, Path);
}
