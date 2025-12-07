using ModOrganizer.Utils;
using System;

namespace ModOrganizer.Mods;

public class ModVirtualFile : IEquatable<ModVirtualFile>
{
    public string Name { get; init; } = string.Empty;
    public string Directory { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;

    public bool Matches(string filter) => TokenMatcher.MatchesMany(filter, [Name, Directory, Path]);
    public override bool Equals(object? obj) => Equals(obj as ModVirtualFile);
    public bool Equals(ModVirtualFile? other) => other != null && GetHashCode() == other.GetHashCode();
    public override int GetHashCode() => Path.GetHashCode();
}
