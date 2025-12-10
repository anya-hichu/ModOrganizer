using ModOrganizer.Windows.States.Results.Visibles;

namespace ModOrganizer.Windows.States.Results.Rules;

public abstract class RuleResult(string currentPath) : Result, IVisibleResult
{
    public string CurrentPath { get; init; } = currentPath;

    public abstract bool IsVisible(IVisibleResultState visibleResultState);
}
