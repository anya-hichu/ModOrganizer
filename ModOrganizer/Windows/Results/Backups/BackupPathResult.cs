using ModOrganizer.Shared;
using ModOrganizer.Windows.Results.Showables;

namespace ModOrganizer.Windows.Results.Backups;

public class BackupPathResult : BackupResult
{
    public string? OldPath { get; init; }

    public override bool IsShowed(IShowableBackupResultState state) => base.IsShowed(state) && TokenMatcher.Matches(state.OldPathFilter, OldPath);
}
