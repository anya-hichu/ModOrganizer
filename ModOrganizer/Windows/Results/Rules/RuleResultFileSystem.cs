using ModOrganizer.Virtuals;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ModOrganizer.Windows.Results.Rules;

public class RuleResultFileSystem : VirtualFileSystem, IRuleResultFileSystem
{
    private RuleResultState RuleResultState { get; init; }

    private HashSet<Result>? MaybeResultsCache { get; set; }

    public RuleResultFileSystem(RuleResultState ruleResultState)
    {
        RuleResultState = ruleResultState;

        RuleResultState.OnResultsChanged += OnResultsChanged;
    }

    public void Dispose() => RuleResultState.OnResultsChanged -= OnResultsChanged;

    private void OnResultsChanged()
    {
        MaybeResultsCache = [.. RuleResultState.GetResults<RulePathResult>()];
        InvalidateRootFolderCache();
    }

    protected override bool TryGetFileList([NotNullWhen(true)] out Dictionary<string, string>? fileList)
    {
        fileList = null;

        if (MaybeResultsCache == null) return false;

        fileList = MaybeResultsCache.OfType<RulePathResult>().ToDictionary(r => r.Directory, r => r.NewPath.Split(PATH_SEPARATOR).Last());
        return true;
    }

    protected override bool TryGetFilePath(string modDirectory, [NotNullWhen(true)] out string? filePath)
    {
        filePath = null;

        if (MaybeResultsCache == null) return false;
        if (!MaybeResultsCache.TryGetValue(new Result() { Directory = modDirectory }, out var result)) return false;
        if (result is not RulePathResult rulePathResult) return false;

        filePath = rulePathResult.NewPath;
        return true;
    }

    public bool TryGetFileData(VirtualFile file, [NotNullWhen(true)] out RulePathResult? fileData) 
    {
        fileData = null;

        if (MaybeResultsCache == null) return false;

        if (!MaybeResultsCache.TryGetValue(new() { Directory = file.Directory }, out var result)) return false;
        if (result is not RulePathResult rulePathResult) return false;

        fileData = rulePathResult;
        return true;
    }
}
