namespace ModOrganizer.Json.Containers;

public record NamedContainer : Container
{
    public string? Name { get; init; }
}
