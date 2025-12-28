using ModOrganizer.Backups.Fakes;

namespace ModOrganizer.Tests.Backups.BackupManagers;

public interface IStubbableBackupManager
{
    StubIBackupManager BackupManagerStub { get; }
}
