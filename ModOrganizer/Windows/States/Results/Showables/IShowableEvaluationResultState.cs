namespace ModOrganizer.Windows.States.Results.Showables;

public interface IShowableEvaluationResultState : IShowableResultState
{
    string ModDirectoryFilter { get; }
    string ExpressionFilter { get; }
    string TemplateFilter { get; }
}
