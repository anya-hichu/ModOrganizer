using Microsoft.QualityTools.Testing.Fakes.Stubs;

namespace ModOrganizer.Tests.Backups.BackupManagers;

public static class IStubbableBackupManagerExtensions
{
    public static T WithBackupManagerDefaults<T>(this T stubbable) where T : IStubbableBackupManager
    {
        stubbable.BackupManagerStub.BehaveAsDefaultValue();

        return stubbable;
    }

    public static T WithBackupManagerObserver<T>(this T stubbable, IStubObserver observer) where T : IStubbableBackupManager
    {
        stubbable.BackupManagerStub.InstanceObserver = observer;

        return stubbable;
    }

}
