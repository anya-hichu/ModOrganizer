using ModOrganizer.Backups;
using ModOrganizer.Windows.Results.Showables;
using System.Threading.Tasks;

namespace ModOrganizer.Windows.Results.Backups;

public interface IBackupResultState : IResultState, IShowableBackupResultState
{
    new string DirectoryFilter { get; set; }

    new bool ShowSamePaths { get; set; }

    new string PathFilter { get; set; }
    new string OldPathFilter { get; set; }

    bool ReloadPenumbra { get; set; }

    bool IsSelected(Backup backup);
    void Select(Backup backup);
    void Unselect();
    Task Preview();
    void Apply();
}
