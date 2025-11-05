using Newtonsoft.Json;
using System.Collections.Generic;

namespace ModOrganizer.Configs;

public class SortOrder
{
    [JsonProperty(Required = Required.Always)]
    public Dictionary<string, string> Data { get; set; } = [];
}
