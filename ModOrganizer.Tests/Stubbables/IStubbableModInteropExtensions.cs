namespace ModOrganizer.Tests.Stubbables;

public static class IStubbableModInteropExtensions
{
    public static T WithModInteropSortOrderPath<T>(this T stubbable, string path, bool exists = false) where T : IStubbableModInterop
    {
        if (exists && !File.Exists(path)) File.WriteAllText(path, string.Empty);
        stubbable.ModInteropStub.GetSortOrderPath = () => path;

        return stubbable;
    }
}
