using ModOrganizer.Windows.States.Results.Selectables;
using ModOrganizer.Windows.States.Results.Visibles;

namespace ModOrganizer.Windows.States.Results.Rules;

public class RulePathResult(string currentPath, string newPath) : RuleResult(currentPath), ISelectableResult
{
    public string NewPath { get; init; } = newPath;
    public bool IsSelected { get; set; } = true;
    public override bool IsVisible(IVisibleResultState _) => true;
}
