using ModOrganizer.Shared;
using ModOrganizer.Windows.States.Results.Showables;

namespace ModOrganizer.Windows.States.Results;

public class EvaluationResult : Result, IShowableResult<IShowableEvaluationResultState>
{
    public string? ExpressionValue { get; set; }
    public Error? ExpressionError { get; set; }
    
    public string? TemplateValue { get; set; }
    public Error? TemplateError { get; set; }

    public bool IsShowed(IShowableEvaluationResultState state)
    {
        if (!base.IsShowed(state)) return false;
        if (ExpressionValue != null && !TokenMatcher.Matches(state.ExpressionFilter, ExpressionValue)) return false;
        if (ExpressionError != null && !TokenMatcher.Matches(state.ExpressionFilter, ExpressionError.Message)) return false;

        if (TemplateValue != null && !TokenMatcher.Matches(state.TemplateFilter, TemplateValue)) return false;
        if (TemplateError != null && !TokenMatcher.Matches(state.TemplateFilter, TemplateError.Message)) return false;

        return true;
    }
}
