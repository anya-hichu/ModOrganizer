using ModOrganizer.Json.Manipulations;
using System.Collections.Generic;

namespace ModOrganizer.Json.Options;

// Where is multi-inheritance when you need it?
public record OptionContainer : Option
{
    public Dictionary<string, string>? Files { get; set; }
    public Dictionary<string, string>? FileSwaps { get; set; }
    public ManipulationWrapper[]? Manipulations { get; set; }
}
