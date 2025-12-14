using ModOrganizer.Shared;
using ModOrganizer.Windows.States.Results.Showables;

namespace ModOrganizer.Windows.States.Results.Backups;

public class BackupSamePathResult : BackupResult
{
    public override bool IsShowed(IShowableBackupResultState state) => state.ShowSamePaths && base.IsShowed(state) && TokenMatcher.Matches(state.NewPathFilter, Path);
}
