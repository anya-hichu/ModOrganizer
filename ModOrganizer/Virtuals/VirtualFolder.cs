using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ModOrganizer.Virtuals;

public class VirtualFolder : IComparable<VirtualFolder>, IEquatable<VirtualFolder>
{
    public string Name { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;

    public HashSet<VirtualFolder> Folders { get; init; } = [];
    public HashSet<VirtualFile> Files { get; init; } = [];

    public bool TrySearch(string filter, [NotNullWhen(true)] out VirtualFolder? result) => TrySearch(new VirtualAttributesMatcher(filter), out result);

    public bool TrySearch(VirtualMatcher matcher, [NotNullWhen(true)] out VirtualFolder? result)
    {
        result = null;

        if (matcher.Matches(this))
        {
            result = this;
            return true;
        }

        var filteredfolder = new VirtualFolder()
        {
            Name = Name,
            Path = Path,
            Folders = [.. Folders.SelectMany<VirtualFolder, VirtualFolder>(f => f.TrySearch(matcher, out var filteredSubfolder) ? [filteredSubfolder] : [])],
            Files = [.. Files.Where(matcher.Matches)]
        };

        if (filteredfolder.IsEmpty()) return false;

        result = filteredfolder;
        return true;
    }
    
    public bool IsEmpty() => Files.Count == 0 && Folders.All(f => f.IsEmpty());

    public IEnumerable<VirtualFile> GetNestedFiles() => Folders.SelectMany(f => f.GetNestedFiles()).Union(Files);

    public int CompareTo(VirtualFolder? other) => StringComparer.OrdinalIgnoreCase.Compare(Name, other?.Name);

    public override bool Equals(object? obj) => Equals(obj as VirtualFile);
    public bool Equals(VirtualFolder? other) => other != null && GetHashCode() == other.GetHashCode();
    public override int GetHashCode() => Path.GetHashCode();
}
