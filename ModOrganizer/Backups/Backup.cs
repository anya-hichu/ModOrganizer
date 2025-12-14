using ModOrganizer.Virtuals;
using System;

namespace ModOrganizer.Backups;

[Serializable]
public class Backup : IComparable<Backup>
{
    public bool Manual { get; set; }
    public DateTime CreatedAt { get; set; }
    public string FileName { get; set; } = string.Empty;

    public int CompareTo(Backup? other) => other == null ? 1 : DateTimeOffset.Compare(CreatedAt, other.CreatedAt);
}
