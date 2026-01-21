namespace ModOrganizer.Virtuals;

public interface IVirtualMatcher
{
    bool Matches(VirtualFile file);
    bool Matches(VirtualFolder folder);
}
