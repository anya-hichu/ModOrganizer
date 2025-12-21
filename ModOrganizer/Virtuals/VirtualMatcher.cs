using System.Linq;

namespace ModOrganizer.Virtuals;

public abstract class VirtualMatcher
{
    public virtual bool Matches(VirtualFolder folder) => folder.Files.All(Matches) && folder.Folders.All(Matches);

    public abstract bool Matches(VirtualFile file);
}
