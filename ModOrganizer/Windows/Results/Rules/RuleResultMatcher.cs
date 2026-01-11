using ModOrganizer.Virtuals;

namespace ModOrganizer.Windows.Results.Rules;

public class RuleResultMatcher(IRuleResultFileSystem ruleResultFileSystem, bool showUnselected) : VirtualMatcher
{
    public override bool MatchesFile(VirtualFile file) => ruleResultFileSystem.TryGetFileData(file, out var rulePathResult) && (rulePathResult.Selected || showUnselected);
}
