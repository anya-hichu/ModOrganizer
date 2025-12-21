using ModOrganizer.Shared;
using System.Text.Json;

namespace ModOrganizer.Json.Writers;

public static class Utf8JsonWriterExtensions
{
    public static RaiiGuard WriteObject(this Utf8JsonWriter writer) => new(writer.WriteStartObject, writer.WriteEndObject);
    public static RaiiGuard WriteArray(this Utf8JsonWriter writer) => new(writer.WriteStartArray, writer.WriteEndArray);
}
