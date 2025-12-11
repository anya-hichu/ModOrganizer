using ModOrganizer.Windows.States.Results.Showables;
using System.Collections.Generic;
using System.Linq;

namespace ModOrganizer.Windows.States.Results.Showables;

public static class IShowableEvaluationResultStateExtensions
{
    public static IReadOnlyDictionary<string, IShowableEvaluationResult> GetShowableResultByModDirectory(this IShowableEvaluationResultState state) => state.GetResultByModDirectory<IShowableEvaluationResult>();
    public static IReadOnlyDictionary<string, IShowableEvaluationResult> GetShowedResultByModDirectory(this IShowableEvaluationResultState state) => state.GetShowableResultByModDirectory().Where(p => p.Value.IsShowed(state)).ToDictionary();
}
