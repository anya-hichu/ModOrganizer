using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ModOrganizer.Configs.Types;

// https://github.com/xivdev/Penumbra/blob/master/schemas/structs/container.json
public class Container
{
    public Dictionary<string, string>? Files { get; set; }
    public Dictionary<string, string>? FileSwaps { get; set; }

    public Manipulation[]? Manipulations { get; set; }
}
