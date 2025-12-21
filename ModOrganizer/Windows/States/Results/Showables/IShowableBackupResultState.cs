namespace ModOrganizer.Windows.States.Results.Showables;

public interface IShowableBackupResultState : IShowableResultState
{
    bool ShowSamePaths { get; }

    string PathFilter { get; }
    string OldPathFilter { get; }
}
