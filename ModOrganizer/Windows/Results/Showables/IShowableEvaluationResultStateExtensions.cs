using ModOrganizer.Windows.Results.Evaluations;
using System.Collections.Generic;

namespace ModOrganizer.Windows.Results.Showables;

public static class IShowableEvaluationResultStateExtensions
{
    public static IEnumerable<EvaluationResult> GetShowedEvaluationResults(this IShowableEvaluationResultState state) => state.GetShowedResults<EvaluationResult, IShowableEvaluationResultState>();
}
