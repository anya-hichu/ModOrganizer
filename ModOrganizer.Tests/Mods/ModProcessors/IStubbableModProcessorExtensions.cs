using Microsoft.QualityTools.Testing.Fakes.Stubs;

namespace ModOrganizer.Tests.Mods.ModProcessors;

public static class IStubbableModProcessorExtensions
{
    public static T WithModProcessorDefaults<T>(this T stubbable) where T : IStubbableModProcessor
    {
        stubbable.ModProcessorStub.BehaveAsDefaultValue();

        return stubbable;
    }

    public static T WithModProcessorObserver<T>(this T stubbable, IStubObserver observer) where T : IStubbableModProcessor
    {
        stubbable.ModProcessorStub.InstanceObserver = observer;

        return stubbable;
    }

    public static T WithModProcessorTryProcess<T>(this T stubbable, string? stubValue) where T : IStubbableModProcessor
    {
        stubbable.ModProcessorStub.TryProcessStringStringOutBoolean = (modDirectory, out newModPath, dryRun) =>
        {
            newModPath = stubValue;
            return stubValue != null;
        };

        return stubbable;
    }
}
