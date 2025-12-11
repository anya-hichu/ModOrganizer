using ModOrganizer.Windows.States.Results.Showables;

namespace ModOrganizer.Windows.States.Results;

public class ErrorResult : Result, IErrorResult, IShowableEvaluationResult
{
    public required string Message { get; init; }
    public string? InnerMessage { get; init; }

    public bool IsShowed(IShowableEvaluationResultState state) => !state.HasFilters();
}
