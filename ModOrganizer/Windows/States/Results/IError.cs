namespace ModOrganizer.Windows.States.Results;

public interface IError
{
    string Message { get; }
    string? InnerMessage { get; }
}
