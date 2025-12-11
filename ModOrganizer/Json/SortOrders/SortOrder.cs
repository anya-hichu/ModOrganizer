using System.Collections.Generic;

namespace ModOrganizer.Json.SortOrders;

// No schema
public class SortOrder
{
    public required Dictionary<string, string> Data { get; set; }
}
