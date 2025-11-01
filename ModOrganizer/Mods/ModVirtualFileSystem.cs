using System;
using System.Linq;

namespace ModOrganizer.Mods;

public class ModVirtualFileSystem : IDisposable
{
    private static readonly char PATH_SEPARATOR = '/';

    private ModInterop ModInterop { get; init; }
    private ModVirtualFolder? MaybeRootFolderCache { get; set; }

    public ModVirtualFileSystem(ModInterop modInterop)
    {
        ModInterop = modInterop;
        ModInterop.OnModsChanged += InvalidateRootFolderCache;
    }

    public void Dispose() => ModInterop.OnModsChanged -= InvalidateRootFolderCache;

    private void InvalidateRootFolderCache() => MaybeRootFolderCache = null;

    public ModVirtualFolder GetRootFolder()
    {
        if (MaybeRootFolderCache != null) return MaybeRootFolderCache;

        var rootFolder = new ModVirtualFolder();
        foreach (var (modDirectory, modName) in ModInterop.GetModList())
        {
            var modPath = ModInterop.GetModPath(modDirectory);
            var segments = modPath.Split(PATH_SEPARATOR);

            var currentParent = rootFolder;
            for (var i = 0; i < segments.Length; i++)
            {
                if (i == segments.Length - 1)
                {
                    currentParent.Files.Add(new() { Directory = modDirectory, Name = modName, Path = modPath });
                }
                else
                {
                    var maybeNewFolder = new ModVirtualFolder() { Name = segments[i], Path = string.Join(PATH_SEPARATOR, segments.Take(i + 1)) };
                    currentParent.Folders.Add(maybeNewFolder);
                    if (currentParent.Folders.TryGetValue(maybeNewFolder, out var existingFolder))
                    {
                        currentParent = existingFolder;
                    }
                }
            }
        }

        return MaybeRootFolderCache = rootFolder;
    }
}
