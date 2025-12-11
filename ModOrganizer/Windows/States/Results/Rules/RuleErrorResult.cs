using ModOrganizer.Windows.States.Results.Showables;

namespace ModOrganizer.Windows.States.Results.Rules;

public class RuleErrorResult : RuleResult, IError
{
    public required string Message { get; init; }
    public string? InnerMessage { get; init; }

    public override bool IsShowed(IShowableRuleResultState state) => state.ShowErrors;
}
