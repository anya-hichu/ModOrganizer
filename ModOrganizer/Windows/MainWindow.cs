using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Lumina.Text.ReadOnly;
using ModOrganizer.Mods;
using ModOrganizer.Utils;
using ModOrganizer.Virtuals;
using ModOrganizer.Windows.States;
using ModOrganizer.Windows.States.Results;
using ModOrganizer.Windows.States.Results.Rules;
using ModOrganizer.Windows.States.Results.Selectables;
using Scriban;
using Scriban.Helpers;
using Scriban.Parsing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;


namespace ModOrganizer.Windows;

public class MainWindow : Window, IDisposable
{
    private ModInterop ModInterop { get; init; }
    private ModFileSystem ModFileSystem { get; init; }
    private IPluginLog PluginLog { get; init; }

    private RuleEvaluationState RuleEvaluationState { get; init; }
    private EvaluationState EvaluationState { get; init; }

    private string Filter { get; set; } = string.Empty;
    private HashSet<string> SelectedModDirectories { get; set; } = [];

    private TemplateContext ViewTemplateContext { get; init; } = new() { MemberRenamer = MemberRenamer.Rename };
    private SourceSpan ViewSourceSpan { get; init; } = new();

    public MainWindow(ModInterop modInterop, ModFileSystem modFileSystem, IPluginLog pluginLog, RuleEvaluationState ruleEvaluationState, Action toggleMainWindow, Action togglePreviewWindow) : base("ModOrganizer - Main##mainWindow")
    {
        SizeConstraints = new()
        {
            MinimumSize = new(375, 330),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };

        TitleBarButtons = [
            new() { Icon = FontAwesomeIcon.Cog, ShowTooltip = () => ImGui.SetTooltip("Toggle config window"), Click = _ => toggleMainWindow() },
            new() { Icon = FontAwesomeIcon.Eye, ShowTooltip = () => ImGui.SetTooltip("Toggle preview window"), Click = _ => togglePreviewWindow() },
        ];

        ModInterop = modInterop;
        ModFileSystem = modFileSystem;
        PluginLog = pluginLog;
        RuleEvaluationState = ruleEvaluationState;

        EvaluationState = new(ModInterop, PluginLog);

        ModInterop.OnModDeleted += OnModDeleted;
        ModInterop.OnModMoved += OnModMoved;
    }

    public void Dispose() 
    {
        ModInterop.OnModDeleted -= OnModDeleted;
        ModInterop.OnModMoved -= OnModMoved;

        ModInterop.ToggleFsWatchers(false);
        RuleEvaluationState.Dispose();
        EvaluationState.Dispose();
    } 

    public override void OnOpen() => ModInterop.ToggleFsWatchers(true);

    public override void OnClose() => ModInterop.ToggleFsWatchers(false);

    private void OnModDeleted(string modDirectory) => SelectedModDirectories.Remove(modDirectory);

    private void OnModMoved(string modDirectory, string newModDirectory)
    {
        if (SelectedModDirectories.Remove(modDirectory)) SelectedModDirectories.Add(newModDirectory);
    }

    private void ToggleFolderSelection(VirtualFolder folder)
    {
        var modDirectories = folder.GetNestedFiles().Select(f => f.Directory);
        SelectedModDirectories = [.. SelectedModDirectories.Overlaps(modDirectories) ? SelectedModDirectories.Except(modDirectories) : SelectedModDirectories.Union(modDirectories)];
    }

    private void DrawVirtualFolderTree(VirtualFolder folder)
    {
        var orderedSubfolders = folder.Folders.OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase);
        foreach (var subfolder in orderedSubfolders)
        {
            var hash = subfolder.GetHashCode();
            using var _ = ImRaii.PushColor(ImGuiCol.Text, Constants.LIGHT_BLUE);
            using var treeNode = ImRaii.TreeNode($"{subfolder.Name}###modVirtualFolder{hash}");
            if (ImGui.IsItemHovered()) ImGui.SetTooltip(subfolder.Name);
            if (ImGui.IsItemClicked() && ImGui.IsKeyDown(ImGuiKey.LeftCtrl)) ToggleFolderSelection(subfolder);
            if (treeNode) DrawVirtualFolderTree(subfolder);
        }

        var orderedFiles = folder.Files.OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase).ToArray();
        for (var i = 0; i < orderedFiles.Length; i++)
        {
            var file = orderedFiles.ElementAt(i);

            var hash = file.GetHashCode();
            using var _ = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudWhite);
            using var __ = ImRaii.TreeNode($"{file.Name}###modVirtualFile{hash}", ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Bullet | (SelectedModDirectories.Contains(file.Directory) ? ImGuiTreeNodeFlags.Selected : ImGuiTreeNodeFlags.None));
            if (ImGui.IsItemHovered()) ImGui.SetTooltip(file.Name);

            if (!ImGui.IsItemClicked()) continue;

            if (ImGui.IsKeyDown(ImGuiKey.LeftShift) && SelectedModDirectories.Count == 1)
            {
                var modDirectories = orderedFiles.Select(f => f.Directory).ToArray();
                var j = Array.IndexOf(modDirectories, SelectedModDirectories.First());
                if (j != -1) SelectedModDirectories.UnionWith(i > j ? modDirectories[j..(i + 1)] : modDirectories[i..(j + 1)]);
                continue;
            } 

            if (!ImGui.IsKeyDown(ImGuiKey.LeftCtrl)) SelectedModDirectories.Clear();

            // Toggle
            if (!SelectedModDirectories.Remove(file.Directory)) SelectedModDirectories.Add(file.Directory);
        }
    }

    public override void Draw()
    {
        var fullRegion = ImGui.GetContentRegionAvail();
        var leftPanelWidth = ImGui.CalcTextSize("NNNNNNNNNNNNNNNNNNNNNN").X;
        using (var leftPanel = ImRaii.Child("leftPanel", new(leftPanelWidth, fullRegion.Y), false, ImGuiWindowFlags.NoScrollbar))
        {
            var leftRegion = ImGui.GetContentRegionAvail();
            var topLeftPanelHeight = ImGui.GetFrameHeightWithSpacing();
            
            var filter = Filter;
            using (var leftTopPanel = ImRaii.Child("topLeftPanel", new(leftRegion.X, topLeftPanelHeight), false, ImGuiWindowFlags.NoScrollbar))
            {
                var clearButtonSize = ImGui.CalcTextSize("NNN");
                ImGui.SetNextItemWidth(leftRegion.X - clearButtonSize.X);
                if (ImGui.InputTextWithHint("##modFilter", Constants.FILTER_HINT, ref filter)) Filter = filter;
                ImGui.SameLine();
                if (ImGui.Button("X##clearModFilter")) Filter = string.Empty;
            }

            var leftBottomRegion = ImGui.GetContentRegionAvail();
            using var leftBottomPanel = ImRaii.Child("bottomLeftPanel", leftBottomRegion);
            if (ModFileSystem.GetRootFolder().TrySearch(filter, out var filteredFolder)) DrawVirtualFolderTree(filteredFolder);
        }

        ImGui.SameLine();

        using var rightPanel = ImRaii.Child("rightPanel", new(fullRegion.X - leftPanelWidth, fullRegion.Y));

        if (SelectedModDirectories.Count == 0)
        {
            ImGui.Text("No mod selected");
            ImGuiComponents.HelpMarker("Click on the left panel to select one and hold <L-CTRL> or <L-SHIFT> (range) for multi-selection");
        }
        else
        {
            using var tabBar = ImRaii.TabBar("selectedMacrosTabBar");

            using (var organizertab = ImRaii.TabItem($"Organizer###organizerTab"))
            {
                if (organizertab) DrawOrganizerTab(); 
            }

            using var inspectorTab = ImRaii.TabItem($"Inspector###inspectorTab");
            if (inspectorTab) DrawInspectorTab();
        }
    }

    private void DrawOrganizerTab()
    {
        if (SelectedModDirectories.Count > 1)
        {
            if (ImGui.Button("Evaluate All##evaluateModDirectories")) RuleEvaluationState.Evaluate(SelectedModDirectories);

            ImGui.SameLine();
            ImGui.Text($"Selection Count: {SelectedModDirectories.Count}");
        }

        var availableRegion = ImGui.GetContentRegionAvail();
        var hasResults = RuleEvaluationState.GetResultByModDirectory().Count > 0;
        using (var selectedModsTable = ImRaii.Table("selectedModsTable", 2, ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY | ImGuiTableFlags.Resizable, hasResults ? new(availableRegion.X, (availableRegion.Y / 2) - (2 * ImGui.GetTextLineHeightWithSpacing())) : availableRegion))
        {
            if (selectedModsTable)
            {
                ImGui.TableSetupColumn($"Mod directory##selectedDirectoryName", ImGuiTableColumnFlags.None, 6);
                ImGui.TableSetupColumn($"Actions##selectedDirectoyActions", ImGuiTableColumnFlags.None, 1);
                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableHeadersRow();

                var clipper = ImGui.ImGuiListClipper();
                var orderedModDirectories = SelectedModDirectories.OrderBy(d => d, StringComparer.OrdinalIgnoreCase).ToList();
                clipper.Begin(orderedModDirectories.Count, ImGui.GetTextLineHeightWithSpacing());

                while (clipper.Step())
                {
                    for (var i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
                    {
                        var modDirectory = orderedModDirectories.ElementAt(i);

                        if (ImGui.TableNextColumn())
                        {
                            ImGui.Text(modDirectory);
                            if (ImGui.IsItemHovered()) ImGui.SetTooltip(modDirectory);
                        }

                        if (ImGui.TableNextColumn())
                        {
                            if (ImGui.Button($"X###deselectModDirectory{i}")) SelectedModDirectories.Remove(modDirectory);
                            ImGui.SameLine();
                            if (ImGui.Button($"Evaluate###evaluateModDirectory{i}")) RuleEvaluationState.Evaluate([modDirectory]);
                        }
                    }
                }
            }
        }

        if (hasResults)
        {
            using (ImRaii.Color? _ = ImRaii.PushColor(ImGuiCol.Button, ImGuiColors.HealerGreen), __ = ImRaii.PushColor(ImGuiCol.Text, Constants.BLACK))
            {
                if (ImGui.Button("Apply Selected##applyRuleEvaluation")) RuleEvaluationState.Apply();
            }
            ImGui.SameLine();
            if (ImGui.Button("Invert Selection##toggleRuleEvaluationSelection")) RuleEvaluationState.InvertResultSelection();
            ImGui.SameLine();
            if (ImGui.Button("Clear Selection##toggleRuleEvaluationSelection")) RuleEvaluationState.ClearResultSelection();
            ImGui.SameLine();

            var showErrors = RuleEvaluationState.ShowErrors;
            if (ImGui.Checkbox("Show Errors##showRulePathErrors", ref showErrors)) RuleEvaluationState.ShowErrors = showErrors;
            ImGui.SameLine();

            var showUnchanging = RuleEvaluationState.ShowUnchanging;
            if (ImGui.Checkbox("Show Unchanging##showUnchangingRulePathResults", ref showUnchanging)) RuleEvaluationState.ShowUnchanging = showUnchanging;

            ImGui.SameLine(availableRegion.X - 100);
            using (ImRaii.PushColor(ImGuiCol.Button, ImGuiColors.DalamudRed))
            {
                if (ImGui.Button("Clear All##clearRuleEvaluationState")) RuleEvaluationState.Clear();
            }  

            using var ruleEvaluationResultsTable = ImRaii.Table("ruleEvaluationResultsTable", 4, ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY | ImGuiTableFlags.Resizable, new(availableRegion.X, availableRegion.Y / 2));
            if (ruleEvaluationResultsTable)
            {
                ImGui.TableSetupColumn($"##ruleEvaluationsSelection", ImGuiTableColumnFlags.None, 1);
                ImGui.TableSetupColumn($"Mod Directory##ruleEvaluationDirectoryName", ImGuiTableColumnFlags.None, 4);
                ImGui.TableSetupColumn($"Current Path##ruleEvaluationCurrentPath", ImGuiTableColumnFlags.None, 4);
                ImGui.TableSetupColumn($"New Path##ruleEvaluationNewPath", ImGuiTableColumnFlags.None, 4);
                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableHeadersRow();

                var orderedResults = RuleEvaluationState.GetVisibleResultByModDirectory().OrderBy(p => p.Key, StringComparer.OrdinalIgnoreCase).ToList();

                var clipper = ImGui.ImGuiListClipper();
                clipper.Begin(orderedResults.Count, ImGui.GetTextLineHeightWithSpacing());
                while (clipper.Step())
                {
                    for (var i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
                    {
                        var (modDirectory, result) = orderedResults.ElementAt(i);
                        if (result is not RuleResult ruleResult) continue;

                        if (ImGui.TableNextColumn() && ruleResult is ISelectableResult selectableResult)
                        {
                            var isSelected = selectableResult.IsSelected;
                            if (ImGui.Checkbox($"###selectResult{selectableResult.GetHashCode()}", ref isSelected)) selectableResult.IsSelected = isSelected;
                        }

                        if (ImGui.TableNextColumn())
                        {
                            ImGui.Text(modDirectory);
                            if (ImGui.IsItemHovered()) ImGui.SetTooltip(ViewTemplateContext.ObjectToString(modDirectory, true));
                        }

                        using var _ = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudGrey3, ruleResult is RuleSamePathResult);

                        if (ImGui.TableNextColumn())
                        {
                            ImGui.Text(ruleResult.CurrentPath);
                            if (ImGui.IsItemHovered()) ImGui.SetTooltip(ViewTemplateContext.ObjectToString(ruleResult.CurrentPath, true));
                        }

                        if (ImGui.TableNextColumn())
                        {
                            switch (ruleResult)
                            {
                                case RulePathResult rulePathResult:
                                    DrawResult(rulePathResult);
                                    break;
                                case RuleSamePathResult ruleSamePathResult:
                                    DrawResult(ruleSamePathResult);
                                    break;
                                case IErrorResult errorResult:
                                    DrawResult(errorResult);
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }

    private void DrawInspectorTab()
    {
        var rightRegion = ImGui.GetContentRegionAvail();

        using (ImRaii.PushColor(ImGuiCol.ChildBg, Constants.LIGHT_BLACK))
        {
            using var topRightPanel = ImRaii.Child("topRightPanel", new(rightRegion.X, rightRegion.Y / 2));
            using var _ = ImRaii.PushIndent();
            foreach (var selectedModDirectory in SelectedModDirectories)
            {
                using var modInfoNode = ImRaii.TreeNode($"{selectedModDirectory}##modInfo{selectedModDirectory.GetHashCode()}");

                using var __ = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudWhite2);
                if (modInfoNode.Success && ModInterop.TryGetModInfo(selectedModDirectory, out var modInfo)) DrawObjectTree(modInfo);
            }
        }

        using var bottomRightPanel = ImRaii.Child("bottomRightPanel", new(rightRegion.X, rightRegion.Y / 2 - ImGui.GetTextLineHeightWithSpacing()));

        var bottomRightButtonsWidth = ImGui.CalcTextSize("NNNNNNNNNNNNNNNNN").X;
        var bottomWidgetSize = new Vector2(rightRegion.X - bottomRightButtonsWidth, ImGui.GetFrameHeightWithSpacing() * 4);

        using (ImRaii.PushColor(ImGuiCol.FrameBg, Constants.LIGHT_BLACK))
        {
            var expression = EvaluationState.Expression;
            if (ImGui.InputTextMultiline("##evaluationExpression", ref expression, ushort.MaxValue, bottomWidgetSize)) EvaluationState.Expression = expression;
        }

        ImGui.SameLine();
        if (ImGui.Button("Evaluate##evaluateExpression")) EvaluationState.Evaluate(SelectedModDirectories);
        ImGui.SameLine();
        if (ImGui.Button("Clear##clearEvaluationState")) EvaluationState.Clear();

        if (EvaluationState.GetResultByModDirectory().Count > 0)
        {
            using var table = ImRaii.Table("evaluationResults", 2, ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY | ImGuiTableFlags.Resizable, ImGui.GetContentRegionAvail());

            if (table)
            {
                ImGui.TableSetupColumn($"Mod Directory###evaluationDirectoryName", ImGuiTableColumnFlags.None, 1);
                ImGui.TableSetupColumn($"Evaluation Result###evaluationResult", ImGuiTableColumnFlags.None, 6);
                ImGui.TableSetupScrollFreeze(0, 2);
                ImGui.TableHeadersRow();

                if (ImGui.TableNextColumn())
                {
                    var modDirectoryFilter = EvaluationState.ModDirectoryFilter;
                    if (ImGui.InputTextWithHint("###evaluationModDirectoryFilter", Constants.FILTER_HINT, ref modDirectoryFilter, ushort.MaxValue)) EvaluationState.ModDirectoryFilter = modDirectoryFilter;
                    ImGui.SameLine();
                    if (ImGui.Button("X###clearEvaluationModDirectoryFilter")) EvaluationState.ModDirectoryFilter = string.Empty;
                }

                if (ImGui.TableNextColumn())
                {
                    var resultFilter = EvaluationState.ResultFilter;
                    if (ImGui.InputTextWithHint("###evaluationResultFilter", Constants.FILTER_HINT, ref resultFilter)) EvaluationState.ResultFilter = resultFilter;
                    ImGui.SameLine();
                    if (ImGui.Button("X###clearEvaluationResultFilter")) EvaluationState.ResultFilter = string.Empty;
                }

                var filteredResults = EvaluationState.GetResultByModDirectory().Where(p => TokenMatcher.Matches(EvaluationState.ModDirectoryFilter, p.Key) && p.Value is EvaluationResult r && TokenMatcher.Matches(EvaluationState.ResultFilter, r.Value)).OrderBy(r => r.Key, StringComparer.OrdinalIgnoreCase).ToList();
                
                var clipper = ImGui.ImGuiListClipper();
                clipper.Begin(filteredResults.Count, ImGui.GetTextLineHeightWithSpacing());
                while (clipper.Step())
                {
                    for (var i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
                    {
                        var (modDirectory, result) = filteredResults.ElementAt(i);

                        if (ImGui.TableNextColumn())
                        {
                            ImGui.Text(modDirectory);
                            if (ImGui.IsItemHovered()) ImGui.SetTooltip(ViewTemplateContext.ObjectToString(modDirectory, true));
                        }

                        if (ImGui.TableNextColumn())
                        {
                            switch (result)
                            {
                                case EvaluationResult evaluationResult:
                                    DrawResult(evaluationResult);
                                    break;
                                case IErrorResult errorResult: 
                                    DrawResult(errorResult); 
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }

    private void DrawObjectTree(object value)
    {
        var accessor = ViewTemplateContext.GetMemberAccessor(value);
        foreach (var member in accessor.GetMembers(ViewTemplateContext, ViewSourceSpan, value))
        {
            if (!accessor.TryGetValue(ViewTemplateContext, ViewSourceSpan, value, member, out var memberValue)) continue;

            DrawMemberTree(member, memberValue, $"object{value.GetHashCode()}");
        }
    }

    private void DrawMemberTree(string name, object? value, string baseId)
    {
        if (value == null)
        {
            using var _ = ImRaii.TreeNode($"{name}: null###{baseId}{name}", ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Bullet);
            return;
        }

        var isEmptyList = value is IList l && l.Count == 0;
        var isEmptyDict = value is IDictionary d && d.Count == 0;

        var valueType = value.GetType();
        var isLeaf = valueType.IsPrimitive || isEmptyList || isEmptyDict;
        var isPrintable = isLeaf || valueType.IsEnum || value is string || value is ReadOnlySeString;

        using var treeNode = ImRaii.TreeNode($"{name}: {(isPrintable ? ViewTemplateContext.ObjectToString(value) : string.Empty)} ({valueType.ScriptPrettyName()})###inspect{value.GetHashCode()}{name}", isLeaf ? ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Bullet : ImGuiTreeNodeFlags.None);
        if (!treeNode) return;

        if (value is IList list)
        {
            for (var i = 0; i < list.Count; i++) DrawMemberTree($"[{i}]", list[i], baseId);
            return;
        } 
        
        DrawObjectTree(value);
    }

    private void DrawResult(RulePathResult rulePathResult)
    {
        ImGui.Text(rulePathResult.NewPath);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(ViewTemplateContext.ObjectToString(rulePathResult.NewPath, true));
    }

    private void DrawResult(RuleSamePathResult ruleSamePathResult)
    {
        ImGui.Text(ruleSamePathResult.CurrentPath);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(ViewTemplateContext.ObjectToString(ruleSamePathResult.CurrentPath, true));
    }

    private void DrawResult(EvaluationResult evaluationResult)
    {
        ImGui.Text(evaluationResult.Value);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(ViewTemplateContext.ObjectToString(evaluationResult.Value, true));
    }

    private static void DrawResult(IErrorResult errorResult)
    {
        using var _ = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudRed);
        ImGui.Text(errorResult.Message);
        if (ImGui.IsItemHovered() && errorResult.InnerMessage != null) ImGui.SetTooltip(errorResult.InnerMessage);
    }

    private static void ThrowInvalidResultType()
    {

    }
}
