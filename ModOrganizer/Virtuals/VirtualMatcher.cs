using System.Linq;

namespace ModOrganizer.Virtuals;

public abstract class VirtualMatcher : IVirtualMatcher
{
    public abstract bool Matches(VirtualFile file);

    public virtual bool Matches(VirtualFolder folder) => folder.Files.All(Matches) && folder.Folders.All(Matches);
}
