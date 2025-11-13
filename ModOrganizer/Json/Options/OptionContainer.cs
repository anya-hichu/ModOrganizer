using ModOrganizer.Json.Manipulations;
using System.Collections.Generic;

namespace ModOrganizer.Json.Options;

// Where is multi-inheritance when you need it?
public record OptionContainer : Option //, Container
{
    public Dictionary<string, string>? Files { get; init; }
    public Dictionary<string, string>? FileSwaps { get; init; }
    public ManipulationWrapper[]? Manipulations { get; init; }
}
