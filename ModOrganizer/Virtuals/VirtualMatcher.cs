using System.Linq;

namespace ModOrganizer.Virtuals;

public abstract class VirtualMatcher
{
    public virtual bool Matches(VirtualFolder folder) => folder.Folders.All(Matches) && folder.Files.All(Matches);

    public abstract bool Matches(VirtualFile file);
}
