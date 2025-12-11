namespace ModOrganizer.Virtuals;

public interface IVirtualFileMatcher
{
    bool IsEnabled();
    bool Matches(VirtualFile file);
}
