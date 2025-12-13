namespace ModOrganizer.Windows.States.Results.Showables;

public interface IShowableEvaluationResultState : IShowableResultState
{
    string ExpressionFilter { get; }
    string TemplateFilter { get; }
}
