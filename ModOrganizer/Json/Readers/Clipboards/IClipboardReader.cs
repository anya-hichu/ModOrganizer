using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Json.Readers.Clipboards;

public interface IClipboardReader
{
    bool TryReadClipboard<T>(string data, [NotNullWhen(true)] out T? instance) where T : class;
}
