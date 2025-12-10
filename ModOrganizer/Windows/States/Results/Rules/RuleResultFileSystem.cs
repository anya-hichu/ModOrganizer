using ModOrganizer.Virtuals;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ModOrganizer.Windows.States.Results.Rules;

public class RuleResultFileSystem : VirtualFileSystem, IDisposable
{
    private RuleEvaluationState RuleEvaluationState { get; init; }

    private IReadOnlyDictionary<string, RulePathResult>? MaybeCache { get; set; }

    public RuleResultFileSystem(RuleEvaluationState ruleEvaluationState)
    {
        RuleEvaluationState = ruleEvaluationState;

        RuleEvaluationState.OnResultsChanged += OnResultsChanged;
    }

    public void Dispose()
    {
        RuleEvaluationState.OnResultsChanged -= OnResultsChanged;
    }

    private void OnResultsChanged()
    {
        MaybeCache = RuleEvaluationState.GetResultByModDirectory<RulePathResult>();
        InvalidateRootFolderCache();
    }

    protected override bool TryGetFileList([NotNullWhen(true)] out Dictionary<string, string>? rulePathResultList)
    {
        rulePathResultList = null;

        if (MaybeCache == null) return false;

        rulePathResultList = MaybeCache.ToDictionary(p => p.Key, p => p.Value.NewPath.Split(PATH_SEPARATOR).Last());
        return true;
    }

    protected override bool TryGetFilePath(string modDirectory, [NotNullWhen(true)] out string? newModPath)
    {
        newModPath = null;

        if (MaybeCache == null) return false;
        if (!MaybeCache.TryGetValue(modDirectory, out var rulePathResult)) return false;

        newModPath = rulePathResult.NewPath;
        return true;
    }

    public bool TryGetFileData(VirtualFile file, [NotNullWhen(true)] out RulePathResult? rulePathResult) {
        rulePathResult = null;

        if (MaybeCache == null) return false;

        return MaybeCache.TryGetValue(file.Directory, out rulePathResult);
    }
}
