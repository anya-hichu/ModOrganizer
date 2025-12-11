using ModOrganizer.Shared;
using ModOrganizer.Windows.States.Results.Showables;

namespace ModOrganizer.Windows.States.Results;

public class EvaluationResult : Result, IShowableEvaluationResult
{
    required public string ExpressionValue { get; init; }
    required public string TemplateValue { get; init; }

    public bool IsShowed(IShowableEvaluationResultState state) => TokenMatcher.Matches(state.ExpressionFilter, ExpressionValue) && TokenMatcher.Matches(state.TemplateFilter, TemplateValue);
}
