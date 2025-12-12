using ModOrganizer.Shared;
using System;

namespace ModOrganizer.Virtuals;

public class VirtualFile : IComparable<VirtualFile>, IEquatable<VirtualFile>
{
    public required string Name { get; init; }
    public required string Directory { get; init; }
    public required string Path { get; init; }

    public int CompareTo(VirtualFile? other) => Constants.ORDER_COMPARER.Compare(Name, other?.Name);

    public override bool Equals(object? obj) => Equals(obj as VirtualFile);
    public bool Equals(VirtualFile? other) => other != null && GetHashCode() == other.GetHashCode();
    public override int GetHashCode() => Path.GetHashCode();
}
