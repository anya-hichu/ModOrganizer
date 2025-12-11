using System.Collections.Generic;
namespace ModOrganizer.Windows.States.Results.Showables;

public static class IShowableRuleResultStateExtensions
{
    public static IReadOnlyDictionary<string, IShowableRuleResult> GetShowedResultByModDirectory(this IShowableRuleResultState state) => state.GetShowedResultByModDirectory<IShowableRuleResult, IShowableRuleResultState>();
}
