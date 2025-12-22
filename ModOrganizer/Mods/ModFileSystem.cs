using ModOrganizer.Virtuals;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Mods;

public class ModFileSystem : VirtualFileSystem, IModFileSystem
{
    private IModInterop ModInterop { get; init; }

    public ModFileSystem(IModInterop modInterop)
    {
        ModInterop = modInterop;
        ModInterop.OnModsChanged += InvalidateRootFolderCache;
    }

    public void Dispose() => ModInterop.OnModsChanged -= InvalidateRootFolderCache;

    protected override bool TryGetFileList([NotNullWhen(true)] out Dictionary<string, string>? modList)
    {
        modList = ModInterop.GetModList();
        return true;
    }

    protected override bool TryGetFilePath(string modDirectory, [NotNullWhen(true)] out string? modPath)
    {
        modPath = ModInterop.GetModPath(modDirectory);
        return true;
    }
}
