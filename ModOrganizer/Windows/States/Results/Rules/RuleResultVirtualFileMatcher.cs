using ModOrganizer.Virtuals;

namespace ModOrganizer.Windows.States.Results.Rules;

public class RuleResultVirtualFileMatcher(RuleResultFileSystem ruleResultFileSystem, bool showUnselected) : IVirtualFileMatcher
{
    public RuleResultFileSystem RuleResultFileSystem { get; init; } = ruleResultFileSystem;
    public bool ShowUnselected { get; init; } = showUnselected;

    public bool IsEnabled() => !ShowUnselected;

    public bool Matches(VirtualFile file) => RuleResultFileSystem.TryGetFileData(file, out var rulePathResult) && rulePathResult.Selected;
}
