namespace ModOrganizer.Windows.Results.Showables;

public interface IShowableRuleResultState: IShowableResultState
{
    bool ShowErrors { get; }
    bool ShowSamePaths { get; }

    string PathFilter { get; }
    string NewPathFilter { get; }
}
