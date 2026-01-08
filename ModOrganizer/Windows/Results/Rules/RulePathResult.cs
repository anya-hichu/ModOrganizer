using ModOrganizer.Shared;
using ModOrganizer.Windows.Results.Selectables;
using ModOrganizer.Windows.Results.Showables;

namespace ModOrganizer.Windows.Results.Rules;

public class RulePathResult : RuleResult, ISelectableResult
{
    public required string NewPath { get; init; }
    public bool Selected { get; set; } = true;
    
    public override bool IsShowed(IShowableRuleResultState state) => base.IsShowed(state) && TokenMatcher.Matches(state.NewPathFilter, NewPath);
}
