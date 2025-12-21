using System.Collections.Generic;

namespace ModOrganizer.Json.Readers.Penumbra.SortOrders;

// No schema
public class SortOrder
{
    public Dictionary<string, string> Data { get; set; } = [];
    public string[] EmptyFolders { get; set; } = [];
}
