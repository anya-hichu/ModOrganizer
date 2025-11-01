using System;
using System.Collections.Generic;

namespace ModOrganizer.Mods;

public class ModVirtualFolder : IEquatable<ModVirtualFolder>
{
    public string Name { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;

    public HashSet<ModVirtualFolder> Folders { get; init; } = [];
    public HashSet<ModVirtualFile> Files { get; init; } = [];

    public override bool Equals(object? obj) => Equals(obj as ModVirtualFile);
    public bool Equals(ModVirtualFolder? other) => other != null && GetHashCode() == other.GetHashCode();
    public override int GetHashCode() => Path.GetHashCode();
}
