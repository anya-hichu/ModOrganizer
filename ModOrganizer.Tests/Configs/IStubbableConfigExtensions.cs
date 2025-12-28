using ModOrganizer.Backups;

namespace ModOrganizer.Tests.Configs;

public static class IStubbableConfigExtensions
{
    public static T WithConfigBackups<T>(this T stubbable, HashSet<Backup> backups) where T : IStubbableConfig
    {
        stubbable.ConfigStub.BackupsGet = () => backups;

        return stubbable;
    }

    public static T WithConfigAutoProcessEnabled<T>(this T stubbable, bool value) where T : IStubbableConfig
    {
        stubbable.ConfigStub.AutoProcessEnabledGet = () => value;

        return stubbable;
    }

    public static T WithConfigAutoProcessDelay<T>(this T stubbable, ushort value) where T : IStubbableConfig
    {
        stubbable.ConfigStub.AutoProcessDelayMsGet = () => value;

        return stubbable;
    }

    public static T WithConfigAutoBackupLimit<T>(this T stubbable, ushort value) where T : IStubbableConfig
    {
        stubbable.ConfigStub.AutoBackupLimitGet = () => value;

        return stubbable;
    }
}
