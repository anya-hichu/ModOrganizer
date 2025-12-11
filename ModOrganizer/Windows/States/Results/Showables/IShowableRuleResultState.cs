namespace ModOrganizer.Windows.States.Results.Showables;

public interface IShowableRuleResultState: IShowableResultState
{
    bool ShowErrors { get; }
    bool ShowSamePaths { get; }
}
