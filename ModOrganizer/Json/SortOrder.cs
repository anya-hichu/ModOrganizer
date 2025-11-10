using System.Collections.Generic;

namespace ModOrganizer.Json;

// No schema
public record SortOrder
{
    public required Dictionary<string, string> Data { get; set; } = [];
}
