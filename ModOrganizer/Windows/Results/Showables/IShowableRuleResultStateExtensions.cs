using ModOrganizer.Windows.Results.Rules;
using System.Collections.Generic;

namespace ModOrganizer.Windows.Results.Showables;

public static class IShowableRuleResultStateExtensions
{
    public static IEnumerable<RuleResult> GetShowedRuleResults(this IShowableRuleResultState state) => state.GetShowedResults<RuleResult, IShowableRuleResultState>();
}
