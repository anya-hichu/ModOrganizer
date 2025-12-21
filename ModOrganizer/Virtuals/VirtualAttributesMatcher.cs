using ModOrganizer.Shared;

namespace ModOrganizer.Virtuals;

public class VirtualAttributesMatcher(string filter) : VirtualMatcher
{
    public override bool Matches(VirtualFile file) => TokenMatcher.MatchesMany(filter, [file.Name, file.Directory, file.Path]);
}
