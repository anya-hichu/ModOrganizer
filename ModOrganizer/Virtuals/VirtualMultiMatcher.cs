using System.Collections.Generic;
using System.Linq;

namespace ModOrganizer.Virtuals;

public class VirtualMultiMatcher(IEnumerable<IVirtualMatcher> matchers) : VirtualMatcher
{
    public override bool Matches(VirtualFile file) => matchers.All(m => m.Matches(file));
    public override bool Matches(VirtualFolder folder) => matchers.All(m => m.Matches(folder));  
}
