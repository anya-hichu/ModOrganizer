using Dalamud.Utility;
using ModOrganizer.Shared;

namespace ModOrganizer.Virtuals;

public class VirtualFileAttributesMatcher(string filter) : IVirtualFileMatcher
{
    private string Filter { get; set; } = filter;

    public bool IsEnabled() => !Filter.IsNullOrWhitespace();

    public bool Matches(VirtualFile file) => TokenMatcher.MatchesMany(Filter, [file.Name, file.Directory, file.Path]);
}
