using System;


namespace ModOrganizer.Backups;

[Serializable]
public class Backup : IComparable<Backup>, IEquatable<Backup>
{
    public bool Auto { get; set; } = false;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public string GetPrettyType() => GetPrettyType(Auto);

    public static string GetPrettyType(bool auto) => auto switch
    {
        true => "auto",
        false => "manual"
    };

    public int CompareTo(Backup? other) => other == null ? 1 : DateTimeOffset.Compare(CreatedAt, other.CreatedAt);

    public override bool Equals(object? obj) => Equals(obj as Backup);
    public bool Equals(Backup? other) => other != null && GetHashCode() == other.GetHashCode();
    public override int GetHashCode() => CreatedAt.GetHashCode();
}
