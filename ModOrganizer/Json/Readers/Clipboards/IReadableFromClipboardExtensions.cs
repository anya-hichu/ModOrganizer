using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Json.Readers.Clipboards;

public static class IReadableFromClipboardExtensions
{
    public static bool TryReadFromClipboard<T>(this IReadableFromClipboard<T> readableClipboard, string data, [NotNullWhen(true)] out T? instance) where T : class
    {
        return readableClipboard.ClipboardReader.TryReadClipboard(data, out instance);
    }
}
