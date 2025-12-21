using System;

namespace ModOrganizer.Backups;

public class BackupCreationException(string? message) : Exception(message)
{
    // Empty
}
