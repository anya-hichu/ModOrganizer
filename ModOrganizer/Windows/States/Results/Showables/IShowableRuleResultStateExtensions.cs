using System.Collections.Generic;
using System.Linq;

namespace ModOrganizer.Windows.States.Results.Showables;

public static class IShowableRuleResultStateExtensions
{
    public static IReadOnlyDictionary<string, IShowableRuleResult> GetShowableResultByModDirectory(this IShowableRuleResultState state) => state.GetResultByModDirectory<IShowableRuleResult>();
    public static IReadOnlyDictionary<string, IShowableRuleResult> GetShowedResultByModDirectory(this IShowableRuleResultState state) => state.GetShowableResultByModDirectory().Where(p => p.Value.IsShowed(state)).ToDictionary();
}
