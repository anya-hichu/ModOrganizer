using ModOrganizer.Windows.States.Results.Showables;

namespace ModOrganizer.Windows.States.Results.Rules;

public class RuleSamePathResult : RuleResult
{
    public override bool IsShowed(IShowableRuleResultState state) => state.ShowSamePaths;
}
