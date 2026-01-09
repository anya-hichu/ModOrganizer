using System;

namespace ModOrganizer.Virtuals;

public class VirtualFile : IComparable<VirtualFile>, IEquatable<VirtualFile>
{
    public string Name { get; init; } = string.Empty;
    public string Directory { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;

    public int CompareTo(VirtualFile? other) => StringComparer.OrdinalIgnoreCase.Compare(Name, other?.Name);

    public override bool Equals(object? obj) => Equals(obj as VirtualFile);
    public bool Equals(VirtualFile? other) => other != null && GetHashCode() == other.GetHashCode();
    public override int GetHashCode() => Path.GetHashCode();
}
