namespace ModOrganizer.Windows.Results;

public class Error : IError
{
    public required string Message { get; init; }
    public string? InnerMessage { get; init; }
}
