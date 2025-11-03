using Dalamud.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ModOrganizer.Mods;

public class ModVirtualFolder : IEquatable<ModVirtualFolder>
{
    public string Name { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;

    public HashSet<ModVirtualFolder> Folders { get; init; } = [];
    public HashSet<ModVirtualFile> Files { get; init; } = [];

    public bool TrySearch(string filter, [NotNullWhen(true)] out ModVirtualFolder? filteredFolder)
    {
        if (filter.IsNullOrWhitespace())
        {
            filteredFolder = this;
            return true;
        }

        filteredFolder = new()
        {
            Name = Name,
            Path = Path,
            Folders = [.. Folders.SelectMany<ModVirtualFolder, ModVirtualFolder>(f => f.TrySearch(filter, out var filteredSubfolder) ? [filteredSubfolder] : [])],
            Files = [.. Files.Where(f => f.Matches(filter))]
        };

        return !filteredFolder.IsEmpty();
    }
    
    public bool IsEmpty() => Files.Count == 0 && Folders.All(f => f.IsEmpty());

    public IEnumerable<ModVirtualFile> GetNestedFiles() => Folders.SelectMany(f => f.GetNestedFiles()).Union(Files);

    public override bool Equals(object? obj) => Equals(obj as ModVirtualFile);
    public bool Equals(ModVirtualFolder? other) => other != null && GetHashCode() == other.GetHashCode();
    public override int GetHashCode() => Path.GetHashCode();
}
