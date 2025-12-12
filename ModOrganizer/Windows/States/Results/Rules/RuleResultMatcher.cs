using ModOrganizer.Virtuals;

namespace ModOrganizer.Windows.States.Results.Rules;

public class RuleResultMatcher(RuleResultFileSystem ruleResultFileSystem, bool showUnselected) : VirtualMatcher
{
    public RuleResultFileSystem RuleResultFileSystem { get; init; } = ruleResultFileSystem;
    public bool ShowUnselected { get; init; } = showUnselected;

    public override bool Matches(VirtualFile file) => RuleResultFileSystem.TryGetFileData(file, out var rulePathResult) && (rulePathResult.Selected || ShowUnselected);
}
