using System.Linq;

namespace ModOrganizer.Virtuals;

public abstract class VirtualMatcher : IVirtualMatcher
{
    public abstract bool MatchesFile(VirtualFile file);

    public virtual bool MatchesFolder(VirtualFolder folder) => folder.Files.All(MatchesFile) && folder.Folders.All(MatchesFolder);
}
