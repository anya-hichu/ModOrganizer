using Microsoft.QualityTools.Testing.Fakes.Stubs;

namespace ModOrganizer.Tests.Shared.PluginLogs;

public static class IStubbablePluginLogExtensions
{
    public static T WithPluginLogDefaults<T>(this T stubbable) where T : IStubbablePluginLog
    {
        stubbable.PluginLogStub.BehaveAsDefaultValue();

        return stubbable;
    }

    public static T WithPluginLogObserver<T>(this T stubbable, IStubObserver observer) where T : IStubbablePluginLog
    {
        stubbable.PluginLogStub.InstanceObserver = observer;

        return stubbable;
    }
}
