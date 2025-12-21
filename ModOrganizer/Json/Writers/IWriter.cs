using System.Text.Json;

namespace ModOrganizer.Json.Writers;

public interface IWriter<T>
{
    bool TryWrite(Utf8JsonWriter jsonWriter, T instance);
}
