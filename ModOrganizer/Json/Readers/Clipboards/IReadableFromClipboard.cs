namespace ModOrganizer.Json.Readers.Clipboards;

public interface IReadableFromClipboard<T> : IReader<T> where T : class
{
    IClipboardReader ClipboardReader { get; }
}
