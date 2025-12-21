using System.Collections.Generic;
using System.Linq;

namespace ModOrganizer.Virtuals;

public class VirtualMultiMatcher(List<VirtualMatcher> matchers) : VirtualMatcher
{
    public override bool Matches(VirtualFolder folder) => matchers.All(m => m.Matches(folder));

    public override bool Matches(VirtualFile file) => matchers.All(m => m.Matches(file));
}
