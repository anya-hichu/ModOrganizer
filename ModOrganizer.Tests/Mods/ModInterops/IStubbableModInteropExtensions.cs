using Microsoft.QualityTools.Testing.Fakes;
using ModOrganizer.Mods;
using Penumbra.Api.Enums;

namespace ModOrganizer.Tests.Mods.ModInterops;

public static class IStubbableModInteropExtensions
{
    public static T WithModInteropGetModList<T>(this T stubbable, Dictionary<string, string> value) where T : IStubbableModInterop
    {
        stubbable.ModInteropStub.GetModList = () => value;

        return stubbable;
    }

    public static T WithModInteropGetModPath<T>(this T stubbable, string value) where T : IStubbableModInterop
    {
        stubbable.ModInteropStub.GetModPathString = modDirectory => value;

        return stubbable;
    }

    public static T WithModInteropGetModPath<T>(this T stubbable, FakesDelegates.Func<string, string> func) where T : IStubbableModInterop
    {
        stubbable.ModInteropStub.GetModPathString = func;

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

    public static T WithModInteropTryGetModInfo<T>(this T stubbable, ModInfo? value) where T : IStubbableModInterop
    {
        stubbable.ModInteropStub.TryGetModInfoStringModInfoOut = (modDirectory, out modInfo) =>
        {
            modInfo = value;
            return value != null;
        };

        return stubbable;
    }
}
