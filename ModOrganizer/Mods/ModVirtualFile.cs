using Dalamud.Utility;
using System;
using System.Linq;

namespace ModOrganizer.Mods;

public class ModVirtualFile : IEquatable<ModVirtualFile>
{
    private static readonly string TOKEN_SEPARATOR = " ";
    private static readonly StringComparison MATCH_FLAGS = StringComparison.InvariantCultureIgnoreCase;
    
    public string Name { get; init; } = string.Empty;
    public string Directory { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;

    public bool Matches(string filter)
    {
        if (filter.IsNullOrWhitespace()) return true;

        var tokens = filter.ToLowerInvariant().Split(TOKEN_SEPARATOR).Where(t => !t.IsNullOrWhitespace());
        return tokens.All(t => Name.Contains(t, MATCH_FLAGS) || Directory.Contains(t, MATCH_FLAGS) || Path.Contains(t, MATCH_FLAGS));
    }

    public override bool Equals(object? obj) => Equals(obj as ModVirtualFile);
    public bool Equals(ModVirtualFile? other) => other != null && GetHashCode() == other.GetHashCode();
    public override int GetHashCode() => Path.GetHashCode();
}
