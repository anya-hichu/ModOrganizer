using ModOrganizer.Windows.States.Results.Visibles;

namespace ModOrganizer.Windows.States.Results.Rules;

public class RuleErrorResult(string currentPath, string message, string? innerMessage = null) : RuleResult(currentPath), IErrorResult
{
    public string Message { get; init; } = message;
    public string? InnerMessage { get; init; } = innerMessage;

    public override bool IsVisible(IVisibleResultState visibleResultState) => visibleResultState.ShowErrors;
}
