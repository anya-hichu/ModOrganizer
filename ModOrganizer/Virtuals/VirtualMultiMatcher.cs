using System.Collections.Generic;
using System.Linq;

namespace ModOrganizer.Virtuals;

public class VirtualMultiMatcher(List<VirtualMatcher> matchers) : VirtualMatcher
{
    private List<VirtualMatcher> Matchers { get; init; } = matchers;

    public override bool Matches(VirtualFolder folder) => Matchers.All(m => m.Matches(folder));

    public override bool Matches(VirtualFile file) => Matchers.All(m => m.Matches(file));
}
