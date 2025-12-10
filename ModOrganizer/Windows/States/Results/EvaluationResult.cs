namespace ModOrganizer.Windows.States.Results;

public class EvaluationResult(string value) : Result
{
    public string Value { get; init; } = value;
}
