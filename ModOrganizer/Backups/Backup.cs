using System;


namespace ModOrganizer.Backups;

[Serializable]
public class Backup : IComparable<Backup>, IEquatable<Backup>
{
    public bool Manual { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public int CompareTo(Backup? other) => other == null ? 1 : DateTimeOffset.Compare(CreatedAt, other.CreatedAt);

    public override bool Equals(object? obj) => Equals(obj as Backup);
    public bool Equals(Backup? other) => other != null && GetHashCode() == other.GetHashCode();
    public override int GetHashCode() => CreatedAt.GetHashCode();
}
