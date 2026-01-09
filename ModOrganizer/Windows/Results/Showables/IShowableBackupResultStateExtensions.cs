using ModOrganizer.Windows.Results.Backups;
using System.Collections.Generic;

namespace ModOrganizer.Windows.Results.Showables;

public static class IShowableBackupResultStateExtensions
{
    public static IEnumerable<BackupResult> GetShowedBackupResults(this IShowableBackupResultState state) => state.GetShowedResults<BackupResult, IShowableBackupResultState>();
}
