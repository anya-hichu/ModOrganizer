using Penumbra.Api.Enums;
using System.Collections.Generic;

namespace ModOrganizer.Mods;

public record ModInfo
{
    public required string Directory { get; init; }
    public required string Path { get; init; }

    public required Dictionary<string, object?> ChangedItems { get; init; }
    public required Dictionary<string, object?> Data { get; init; }

    public required Dictionary<string, object?> Default { get; init; }
    public required List<Dictionary<string, object?>> Groups { get; init; }
    public required Dictionary<string, object?> Meta { get; init; }
}
