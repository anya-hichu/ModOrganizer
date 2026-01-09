namespace ModOrganizer.Virtuals;

public interface IVirtualMatcher
{
    bool MatchesFile(VirtualFile file);
    bool MatchesFolder(VirtualFolder folder);
}
