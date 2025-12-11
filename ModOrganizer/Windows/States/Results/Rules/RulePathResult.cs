using ModOrganizer.Windows.States.Results.Selectables;
using ModOrganizer.Windows.States.Results.Showables;

namespace ModOrganizer.Windows.States.Results.Rules;

public class RulePathResult : RuleResult, ISelectableResult
{
    public required string NewPath { get; init; }
    public bool IsSelected { get; set; } = true;
    
    public override bool IsShowed(IShowableRuleResultState _) => true;
}
