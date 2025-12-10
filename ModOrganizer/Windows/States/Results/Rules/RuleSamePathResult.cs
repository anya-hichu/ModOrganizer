using ModOrganizer.Windows.States.Results.Visibles;

namespace ModOrganizer.Windows.States.Results.Rules;

public class RuleSamePathResult(string currentPath) : RuleResult(currentPath)
{
    public override bool IsVisible(IVisibleResultState visibleResultState) => visibleResultState.ShowUnchanging;
}
