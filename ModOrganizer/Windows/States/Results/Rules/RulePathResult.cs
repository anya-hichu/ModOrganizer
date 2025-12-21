using ModOrganizer.Shared;
using ModOrganizer.Windows.States.Results.Selectables;
using ModOrganizer.Windows.States.Results.Showables;

namespace ModOrganizer.Windows.States.Results.Rules;

public class RulePathResult : RuleResult, ISelectableResult
{
    public required string NewPath { get; init; }
    public bool Selected { get; set; } = true;
    
    public override bool IsShowed(IShowableRuleResultState state) => base.IsShowed(state) && TokenMatcher.Matches(state.NewPathFilter, NewPath);
}
