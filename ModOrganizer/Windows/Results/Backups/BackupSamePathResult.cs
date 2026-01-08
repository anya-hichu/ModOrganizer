using ModOrganizer.Shared;
using ModOrganizer.Windows.Results.Showables;

namespace ModOrganizer.Windows.Results.Backups;

public class BackupSamePathResult : BackupResult
{
    public override bool IsShowed(IShowableBackupResultState state) => state.ShowSamePaths && base.IsShowed(state) && TokenMatcher.Matches(state.OldPathFilter, Path);
}
