using ModOrganizer.Shared;

namespace ModOrganizer.Virtuals;

public class VirtualAttributesMatcher(string filter) : VirtualMatcher
{
    private string Filter { get; set; } = filter;

    public override bool Matches(VirtualFile file) => TokenMatcher.MatchesMany(Filter, [file.Name, file.Directory, file.Path]);
}
