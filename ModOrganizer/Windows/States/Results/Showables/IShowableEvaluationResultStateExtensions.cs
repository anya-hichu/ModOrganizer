using System.Collections.Generic;

namespace ModOrganizer.Windows.States.Results.Showables;

public static class IShowableEvaluationResultStateExtensions
{
    public static IReadOnlyDictionary<string, IShowableEvaluationResult> GetShowedResultByModDirectory(this IShowableEvaluationResultState state) => state.GetShowedResultByModDirectory<IShowableEvaluationResult, IShowableEvaluationResultState>();
}
