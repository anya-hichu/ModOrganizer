using ModOrganizer.Backups.Fakes;

namespace ModOrganizer.Tests.Backups;

public interface IStubbableBackupManager
{
    StubIBackupManager BackupManagerStub { get; }
}
