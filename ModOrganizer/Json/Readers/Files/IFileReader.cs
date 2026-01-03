using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Json.Readers.Files;

public interface IFileReader
{
    bool TryReadFile<T>(string path, [NotNullWhen(true)] out T? instance);
}
