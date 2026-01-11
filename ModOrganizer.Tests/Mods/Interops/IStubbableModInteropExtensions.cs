using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.QualityTools.Testing.Fakes.Stubs;
using ModOrganizer.Mods;
using Penumbra.Api.Enums;

namespace ModOrganizer.Tests.Mods.Interops;

public static class IStubbableModInteropExtensions
{
    public static T WithModInteropObserver<T>(this T stubbable, IStubObserver observer) where T : IStubbableModInterop
    {
        stubbable.ModInteropStub.InstanceObserver = observer;

        return stubbable;
    }

    public static T WithModInteropGetModList<T>(this T stubbable, Dictionary<string, string> stubValue) where T : IStubbableModInterop
    {
        stubbable.ModInteropStub.GetModList = () => stubValue;

        return stubbable;
    }

    public static T WithModInteropGetModPath<T>(this T stubbable, string stubValue) where T : IStubbableModInterop
    {
        stubbable.ModInteropStub.GetModPathString = modDirectory => stubValue;

        return stubbable;
    }

    public static T WithModInteropGetModPath<T>(this T stubbable, FakesDelegates.Func<string, string> stubFunc) where T : IStubbableModInterop
    {
        stubbable.ModInteropStub.GetModPathString = stubFunc;

        return stubbable;
    }

    public static T WithModInteropSetModPath<T>(this T stubbable, PenumbraApiEc exitCode) where T : IStubbableModInterop
    {
        stubbable.ModInteropStub.SetModPathStringString = (modDirectory, newModPath) => exitCode;

        return stubbable;
    }

    public static T WithModInteropSortOrderPath<T>(this T stubbable, string path, bool exists = false) where T : IStubbableModInterop
    {
        if (exists && !File.Exists(path)) File.WriteAllText(path, string.Empty);
        stubbable.ModInteropStub.GetSortOrderPath = () => path;

        return stubbable;
    }

    public static T WithModInteropTryGetModInfo<T>(this T stubbable, ModInfo? stubValue) where T : IStubbableModInterop
    {
        stubbable.ModInteropStub.TryGetModInfoStringModInfoOut = (modDirectory, out instance) =>
        {
            instance = stubValue;
            return stubValue != null;
        };

        return stubbable;
    }

    public static T WithModInteropReloadPenumbra<T>(this T stubbable, bool stubValue) where T : IStubbableModInterop
    {
        stubbable.ModInteropStub.ReloadPenumbra = () => stubValue;

        return stubbable;
    }
}
