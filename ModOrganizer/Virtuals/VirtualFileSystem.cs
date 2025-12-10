using FFXIVClientStructs.FFXIV.Client.UI;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ModOrganizer.Virtuals;

public abstract class VirtualFileSystem
{
    protected static readonly char PATH_SEPARATOR = '/';

    private VirtualFolder? MaybeRootFolderCache { get; set; }

    protected void InvalidateRootFolderCache() => MaybeRootFolderCache = null;

    public VirtualFolder GetRootFolder()
    {
        if (MaybeRootFolderCache != null) return MaybeRootFolderCache;

        var rootFolder = new VirtualFolder();
        if (!TryGetFileList(out var fileList)) return rootFolder;

        foreach (var (fileDirectory, fileName) in fileList)
        {
            if (!TryGetFilePath(fileDirectory, out var filePath)) continue;

            var segments = filePath.Split(PATH_SEPARATOR);

            var currentParent = rootFolder;
            for (var i = 0; i < segments.Length; i++)
            {
                if (i == segments.Length - 1)
                {
                    currentParent.Files.Add(new() { Directory = fileDirectory, Name = fileName, Path = filePath });
                }
                else
                {
                    var maybeNewFolder = new VirtualFolder() { Name = segments[i], Path = string.Join(PATH_SEPARATOR, segments.Take(i + 1)) };
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

    protected abstract bool TryGetFileList([NotNullWhen(true)] out Dictionary<string, string>? fileList);
    protected abstract bool TryGetFilePath(string fileDirectory, [NotNullWhen(true)] out string? filePath);
}
