using ModOrganizer.Virtuals;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ModOrganizer.Windows.States.Results.Rules;

public class RuleResultFileSystem : VirtualFileSystem, IDisposable
{
    private RuleEvaluationState RuleEvaluationState { get; init; }

    private IReadOnlyDictionary<string, RulePathResult> RulePathResultByModDirectoryCache { get; set; }

    public RuleResultFileSystem(RuleEvaluationState ruleEvaluationState)
    {
        RuleEvaluationState = ruleEvaluationState;
        RulePathResultByModDirectoryCache = RuleEvaluationState.GetRulePathResultByModDirectory();

        RuleEvaluationState.OnResultsChanged += OnResultsChanged;
    }

    public void Dispose()
    {
        RuleEvaluationState.OnResultsChanged -= OnResultsChanged;
    }

    private void OnResultsChanged()
    {
        RulePathResultByModDirectoryCache = RuleEvaluationState.GetRulePathResultByModDirectory();
        InvalidateRootFolderCache();
    }

    protected override bool TryGetFileList([NotNullWhen(true)] out Dictionary<string, string>? rulePathResultList)
    {
        rulePathResultList = RulePathResultByModDirectoryCache.ToDictionary(p => p.Key, p => p.Value.NewPath.Split(PATH_SEPARATOR).Last());
        return true;
    }

    protected override bool TryGetFilePath(string modDirectory, [NotNullWhen(true)] out string? newModPath)
    {
        newModPath = null;
        if (!RulePathResultByModDirectoryCache.TryGetValue(modDirectory, out var rulePathResult)) return false;

        newModPath = rulePathResult.NewPath;
        return true;
    }

    public bool IsSelected(string modDirectory) => RulePathResultByModDirectoryCache.TryGetValue(modDirectory, out var rulePathResult) && rulePathResult.IsSelected;
}
