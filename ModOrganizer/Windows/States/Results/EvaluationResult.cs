using Dalamud.Utility;
using ModOrganizer.Shared;
using ModOrganizer.Windows.States.Results.Showables;

namespace ModOrganizer.Windows.States.Results;

public class EvaluationResult : Result, IShowableEvaluationResult
{
    public string? ExpressionValue { get; set; }
    public Error? ExpressionError { get; set; }
    
    public string? TemplateValue { get; set; }
    public Error? TemplateError { get; set; }

    public bool IsShowed(IShowableEvaluationResultState state) {
        if (ExpressionError == null && !TokenMatcher.Matches(state.ExpressionFilter, ExpressionValue)) return false;
        if (TemplateError == null && !TokenMatcher.Matches(state.TemplateFilter, TemplateValue)) return false;

        return true;
    }
}
