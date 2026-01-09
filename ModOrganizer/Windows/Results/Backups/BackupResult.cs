using ModOrganizer.Shared;
using ModOrganizer.Windows.Results.Showables;

namespace ModOrganizer.Windows.Results.Backups;

public class BackupResult : Result, IShowableResult<IShowableBackupResultState>
{
    public string? Path { get; init; }

    public virtual bool IsShowed(IShowableBackupResultState state) => base.IsShowed(state) && TokenMatcher.Matches(state.OldPathFilter, Path);
}
