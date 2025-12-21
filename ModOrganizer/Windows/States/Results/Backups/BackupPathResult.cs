using ModOrganizer.Shared;
using ModOrganizer.Windows.States.Results.Showables;

namespace ModOrganizer.Windows.States.Results.Backups;

public class BackupPathResult : BackupResult
{
    public string? OldPath { get; init; }

    public override bool IsShowed(IShowableBackupResultState state) => base.IsShowed(state) && TokenMatcher.Matches(state.OldPathFilter, OldPath);
}
