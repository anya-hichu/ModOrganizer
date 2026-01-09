using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Configs;

namespace ModOrganizer.Tests.Configs.Defaults;

public static class IStubbableConfigDefaultExtensions
{
    public static T WithConfigDefault<T>(this T stubbable, IConfig stubValue) where T : IStubbableConfigDefault
    {
        stubbable.ConfigDefaultStub.Build = () => stubValue;

        return stubbable;
    }
}
