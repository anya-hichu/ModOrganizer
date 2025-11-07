using System.Collections.Generic;

namespace ModOrganizer.Json;

public record SortOrder
{
    public required Dictionary<string, string> Data { get; set; } = [];
}
