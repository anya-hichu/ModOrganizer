namespace ModOrganizer.Windows.Results.Showables;

public interface IShowableEvaluationResultState : IShowableResultState
{
    string ExpressionFilter { get; }
    string TemplateFilter { get; }
}
