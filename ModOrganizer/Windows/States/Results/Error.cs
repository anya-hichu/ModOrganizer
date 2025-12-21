namespace ModOrganizer.Windows.States.Results;

public class Error : IError
{
    public required string Message { get; init; }
    public string? InnerMessage { get; init; }
}
