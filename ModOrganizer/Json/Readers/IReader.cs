using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Readers;

public interface IReader<T> where T : class
{
    bool TryRead(JsonElement jsonElement, [NotNullWhen(true)] out T? instance);
}
