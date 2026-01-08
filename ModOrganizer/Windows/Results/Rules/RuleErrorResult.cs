using ModOrganizer.Shared;
using ModOrganizer.Windows.Results;
using ModOrganizer.Windows.Results.Showables;

namespace ModOrganizer.Windows.Results.Rules;

public class RuleErrorResult : RuleResult, IError
{
    public required string Message { get; init; }
    public string? InnerMessage { get; init; }

    public override bool IsShowed(IShowableRuleResultState state) => state.ShowErrors && base.IsShowed(state) && TokenMatcher.Matches(state.NewPathFilter, Message);
}
