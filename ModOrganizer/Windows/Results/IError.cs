namespace ModOrganizer.Windows.Results;

public interface IError
{
    string Message { get; }
    string? InnerMessage { get; }
}
