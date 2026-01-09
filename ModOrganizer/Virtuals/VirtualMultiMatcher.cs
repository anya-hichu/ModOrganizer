using System.Collections.Generic;
using System.Linq;

namespace ModOrganizer.Virtuals;

public class VirtualMultiMatcher(IEnumerable<IVirtualMatcher> matchers) : VirtualMatcher
{
    public override bool MatchesFile(VirtualFile file) => matchers.All(m => m.MatchesFile(file));
    public override bool MatchesFolder(VirtualFolder folder) => matchers.All(m => m.MatchesFolder(folder));  
}
