using ModOrganizer.Mods;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Rules;

public interface IRuleEvaluator
{
    bool TryEvaluate(IEnumerable<Rule> rules, ModInfo modInfo, [NotNullWhen(true)] out string? path);
    bool TryEvaluate(Rule rule, ModInfo modInfo, [NotNullWhen(true)] out string? path);
}
