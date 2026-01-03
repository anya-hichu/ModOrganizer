using ModOrganizer.Tests.Json.Readers.Files;
using ModOrganizer.Tests.ShimsContexts;
using ModOrganizer.Tests.Systems.DateTimeOffsets;

namespace ModOrganizer.Tests.Backups.BackupManagers;

public class BackupManagerShimsContextBuilder : ShimsContextBuilder, IShimmableDateTimeOffset, IShimmableFileReader
{
    // Empty
}
