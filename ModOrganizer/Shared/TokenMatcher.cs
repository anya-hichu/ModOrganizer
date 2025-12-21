using Dalamud.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModOrganizer.Shared;

public static class TokenMatcher
{
    public static bool Matches(string filter, string? maybeText) => MatchesMany(filter, [maybeText]);

    public static bool MatchesMany(string filter, HashSet<string?> maybeTexts)
    {
        if (filter.IsNullOrWhitespace()) return true;

        var tokens = filter.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return tokens.All(token => maybeTexts.Any(maybeText => maybeText != null && maybeText.Contains(token, StringComparison.InvariantCultureIgnoreCase)));
    }
}
