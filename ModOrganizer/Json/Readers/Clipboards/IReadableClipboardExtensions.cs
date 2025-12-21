using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Json.Readers.Clipboards;

public static class IReadableClipboardExtensions
{
    public static bool TryReadFromClipboard<T>(this IReadableClipboard<T> readableClipboard, string data, [NotNullWhen(true)] out T? instance) where T : class
    {
        return readableClipboard.ClipboardReader.TryReadClipboard(data, out instance);
    }
}
