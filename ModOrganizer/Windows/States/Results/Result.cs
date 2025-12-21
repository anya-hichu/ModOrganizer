using ModOrganizer.Shared;
using ModOrganizer.Windows.States.Results.Showables;
using System;

namespace ModOrganizer.Windows.States.Results;

public class Result : IComparable<Result>, IEquatable<Result>, IShowableResult<IShowableResultState>
{
    public required string Directory { get; set; }

    public virtual bool IsShowed(IShowableResultState state) => TokenMatcher.Matches(state.DirectoryFilter, Directory);

    public int CompareTo(Result? other) => StringComparer.OrdinalIgnoreCase.Compare(Directory, other?.Directory);

    public override bool Equals(object? obj) => Equals(obj as Result);
    public bool Equals(Result? other) => other != null && GetHashCode() == other.GetHashCode();
    public override int GetHashCode() => Directory.GetHashCode();
}
