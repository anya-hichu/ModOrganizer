using ModOrganizer.Json.Penumbra.Manipulations.Wrappers;
using System.Collections.Generic;

namespace ModOrganizer.Json.Penumbra.Options.Containers;

public class OptionContainer : Option
{
    public Dictionary<string, string>? Files { get; init; }
    public Dictionary<string, string>? FileSwaps { get; init; }
    public ManipulationWrapper[]? Manipulations { get; init; }
}
