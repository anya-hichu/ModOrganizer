using ModOrganizer.Json.Readers.Penumbra.Manipulations;
using System.Collections.Generic;

namespace ModOrganizer.Json.Readers.Penumbra.Containers;

// https://github.com/xivdev/Penumbra/blob/318a41fe52ad00ce120d08b2c812e11a6a9b014a/schemas/structs/container.json
public class Container : Data
{
    public Dictionary<string, string>? Files { get; init; }
    public Dictionary<string, string>? FileSwaps { get; init; }
    public ManipulationWrapper[]? Manipulations { get; init; }
}
