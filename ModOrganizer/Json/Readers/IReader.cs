using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers;

public interface IReader<T> where T : class
{
    bool TryRead(JsonElement element, [NotNullWhen(true)] out T? instance);

    bool TryReadMany(JsonElement element, [NotNullWhen(true)] out T[]? instances);
}
