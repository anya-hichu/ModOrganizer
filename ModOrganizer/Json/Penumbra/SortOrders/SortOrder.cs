using System.Collections.Generic;

namespace ModOrganizer.Json.Penumbra.SortOrders;

// No schema
public class SortOrder : Data
{
    public Dictionary<string, string> Data { get; set; } = [];
    public string[] EmptyFolders { get; set; } = [];
}
