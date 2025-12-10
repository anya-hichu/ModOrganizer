namespace ModOrganizer.Windows.States.Results;

public interface IErrorResult
{
    string Message { get; }
    string? InnerMessage { get; }
}
