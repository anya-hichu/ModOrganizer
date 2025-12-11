using System.Collections.Generic;
using System.Linq;

namespace ModOrganizer.Virtuals;

public class VirtualFileMultiMatcher(List<IVirtualFileMatcher> matchers) : IVirtualFileMatcher
{
    private List<IVirtualFileMatcher> Matchers { get; init; } = matchers;

    public bool IsEnabled() => Matchers.Any(m => m.IsEnabled());

    public bool Matches(VirtualFile file) => Matchers.Where(m => m.IsEnabled()).All(m => m.Matches(file));
}
