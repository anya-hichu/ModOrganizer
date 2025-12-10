using System;

namespace ModOrganizer.Windows.States.Results;

public class ErrorResult(string message, string? innerMessage = null) : Result, IErrorResult
{
    public string Message { get; init; } = message;
    public string? InnerMessage { get; init; } = innerMessage;
}
