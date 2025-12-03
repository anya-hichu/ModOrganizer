using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json;

public interface IBuilder<T> where T : class
{
    bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out T? instance);
}
