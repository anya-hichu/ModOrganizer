using Microsoft.QualityTools.Testing.Fakes;

namespace ModOrganizer.Tests.Stubbables;

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

    public static T WithModInteropSortOrderPath<T>(this T stubbable, string path, bool exists = false) where T : IStubbableModInterop
    {
        if (exists && !File.Exists(path)) File.WriteAllText(path, string.Empty);
        stubbable.ModInteropStub.GetSortOrderPath = () => path;

        return stubbable;
    }
}
