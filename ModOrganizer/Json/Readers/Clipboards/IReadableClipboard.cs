namespace ModOrganizer.Json.Readers.Clipboards;

public interface IReadableClipboard<T> : IReader<T> where T : class
{
    ClipboardReader ClipboardReader { get; }
}
