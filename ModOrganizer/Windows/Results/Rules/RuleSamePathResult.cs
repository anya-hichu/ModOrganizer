using ModOrganizer.Shared;
using ModOrganizer.Windows.Results.Showables;

namespace ModOrganizer.Windows.Results.Rules;

public class RuleSamePathResult : RuleResult
{
    public override bool IsShowed(IShowableRuleResultState state) => state.ShowSamePaths && base.IsShowed(state) && TokenMatcher.Matches(state.NewPathFilter, Path);
}
