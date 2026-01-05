using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Lumina.Text.ReadOnly;
using ModOrganizer.Configs;
using ModOrganizer.Mods;
using ModOrganizer.Shared;
using ModOrganizer.Virtuals;
using ModOrganizer.Windows.States;
using ModOrganizer.Windows.States.Results;
using ModOrganizer.Windows.States.Results.Evaluations;
using ModOrganizer.Windows.States.Results.Rules;
using ModOrganizer.Windows.States.Results.Selectables;
using ModOrganizer.Windows.States.Results.Showables;
using Scriban;
using Scriban.Helpers;
using Scriban.Parsing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;


namespace ModOrganizer.Windows;

public class MainWindow : Window, IDisposable
{
    private IConfig Config { get; init; }
    private IModInterop ModInterop { get; init; }
    private IModFileSystem ModFileSystem { get; init; }
    private IPluginLog PluginLog { get; init; }
    private Action TogglePreviewUI { get; init; }
    private Action ToggleBackupUI { get; init; }

    private RuleState RuleState { get; init; }
    private EvaluationState EvaluationState { get; init; }

    private string Filter { get; set; } = string.Empty;
    private HashSet<string> SelectedModDirectories { get; set; } = [];

    private TemplateContext ViewTemplateContext { get; init; } = new() { MemberRenamer = MemberRenamer.Rename };
    private SourceSpan ViewSourceSpan { get; init; } = new();

    public MainWindow(IConfig config, IModInterop modInterop, IModFileSystem modFileSystem, IPluginLog pluginLog, RuleState ruleState, Action toggleBackupUI, Action toggleMainUI, Action togglePreviewUI) : base("ModOrganizer - Main##mainWindow")
    {
        SizeConstraints = new()
        {
            MinimumSize = new(1200, 850),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };

        TitleBarButtons = [
            new(){ 
                Icon = FontAwesomeIcon.Cog, 
                ShowTooltip = () => ImGui.SetTooltip("Toggle config window"), 
                Click = _ => toggleMainUI.Invoke() 
            },
            new() {
                Icon = FontAwesomeIcon.Database,
                ShowTooltip = () => ImGui.SetTooltip("Toggle backup window"),
                Click = _ => toggleBackupUI.Invoke()
            },
            new() {
                Icon = FontAwesomeIcon.Eye,
                ShowTooltip = () => ImGui.SetTooltip("Toggle preview window"),
                Click = _ => togglePreviewUI.Invoke()
            }
        ];

        Config = config;
        ModInterop = modInterop;
        ModFileSystem = modFileSystem;
        PluginLog = pluginLog;
        RuleState = ruleState;
        TogglePreviewUI = togglePreviewUI;
        ToggleBackupUI = toggleBackupUI;

        EvaluationState = new(ModInterop, PluginLog);

        ModInterop.OnModDeleted += OnModDeleted;
        ModInterop.OnModMoved += OnModMoved;
    }

    public void Dispose() 
    {
        ModInterop.OnModDeleted -= OnModDeleted;
        ModInterop.OnModMoved -= OnModMoved;

        ModInterop.ToggleFsWatchers(false);
        RuleState.Dispose();
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
        // TODO: Add possibility to select all
        foreach (var subfolder in folder.Folders.Order())
        {
            var hash = subfolder.GetHashCode();
            using var _ = ImRaii.PushColor(ImGuiCol.Text, CustomColors.LightBlue);
            using var treeNode = ImRaii.TreeNode($"{subfolder.Name}###modVirtualFolder{hash}");
            if (ImGui.IsItemHovered()) ImGui.SetTooltip(subfolder.Name);
            if (ImGui.IsItemClicked() && ImGui.IsKeyDown(ImGuiKey.LeftCtrl)) ToggleFolderSelection(subfolder);
            if (treeNode) DrawVirtualFolderTree(subfolder);
        }

        var orderedFiles = folder.Files.Order().ToArray();
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
                var clearButtonWidth = ImGui.CalcTextSize("NNN").X;
                ImGui.SetNextItemWidth(leftRegion.X - clearButtonWidth);
                if (ImGui.InputTextWithHint("##modFilter", Texts.FilterHint, ref filter)) Filter = filter;
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
            using (ImRaii.Color? _ = ImRaii.PushColor(ImGuiCol.Button, CustomColors.LightBlue), __ = ImRaii.PushColor(ImGuiCol.Text, CustomColors.Black))
            {
                if (ImGui.Button("Preview All##evaluateModDirectories")) RuleState.Preview(SelectedModDirectories);
            } 

            ImGui.SameLine();
            ImGui.Text($"Selection Count: {SelectedModDirectories.Count}");
        }

        var availableRegion = ImGui.GetContentRegionAvail();
        var hasResults = RuleState.GetResults().Any();

        using (var selectedModsDirectoriesTable = ImRaii.Table("selectedModsDirectoriesTable", 2, ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY | ImGuiTableFlags.Resizable, hasResults ? new(availableRegion.X, (availableRegion.Y / 2) - (2 * ImGui.GetTextLineHeightWithSpacing())) : availableRegion))
        {
            if (selectedModsDirectoriesTable)
            {
                ImGui.TableSetupColumn($"Mod Directory##selectedModDirectoryNames", ImGuiTableColumnFlags.None, 6);
                ImGui.TableSetupColumn($"Actions##selectedModDirectoryActions", ImGuiTableColumnFlags.None, 1);
                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableHeadersRow();

                var orderedModDirectories = SelectedModDirectories.OrderBy(d => d, StringComparer.OrdinalIgnoreCase).ToList();

                using var clipperResource = new ImRaiiListClipper(orderedModDirectories.Count, GetItemHeight());
                var clipper = clipperResource.Value;

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
                            if (ImGui.Button($"Deselect###deselectModDirectory{i}")) SelectedModDirectories.Remove(modDirectory);
                            ImGui.SameLine();
                            if (ImGui.Button($"Preview###evaluateModDirectory{i}")) RuleState.Preview([modDirectory]);
                        }
                    }
                }
            }
        }

        if (hasResults)
        {
            var isConfirmPressed = ImGui.GetIO().KeyCtrl;
            var selectedCount = RuleState.GetSelectedResults().Count();

            using (ImRaii.Disabled(!isConfirmPressed))
            {
                using ImRaii.Color? _ = ImRaii.PushColor(ImGuiCol.Button, ImGuiColors.ParsedGreen), __ = ImRaii.PushColor(ImGuiCol.Text, CustomColors.Black);
                if (ImGui.Button($"Apply Selected ({selectedCount})##applyRuleState")) RuleState.Apply();
            }
            if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled)) ImGui.SetTooltip(Texts.ConfirmHint);

            using (ImRaii.Disabled(selectedCount == 0))
            {
                ImGui.SameLine();
                if (ImGui.Button("Clear Selection##clearRuleStateSelection")) RuleState.ClearResultSelection();
            }

            ImGui.SameLine();
            using (ImRaii.Disabled(!RuleState.GetSelectableResults().Any()))
            {
                if (ImGui.Button("Invert Selection##invertRuleStateSelection")) RuleState.InvertResultSelection();
            }

            ImGui.SameLine();
            var showErrors = RuleState.ShowErrors;
            if (ImGui.Checkbox("Show Errors##showRuleStateErrors", ref showErrors)) RuleState.ShowErrors = showErrors;
            ImGui.SameLine();

            var showSamePaths = RuleState.ShowSamePaths;
            if (ImGui.Checkbox("Show Same Paths##showRuleStateSameRule", ref showSamePaths)) RuleState.ShowSamePaths = showSamePaths;

            ImGui.SameLine(availableRegion.X - 300);
            if (ImGui.Button("Backup##toggleBackupUI")) ToggleBackupUI();

            ImGui.SameLine();
            if (ImGui.Button("Preview##togglePreviewUI")) TogglePreviewUI();

            ImGui.SameLine();
            using (ImRaii.PushColor(ImGuiCol.Button, ImGuiColors.DalamudRed))
            {
                if (ImGui.Button("Clear All##clearRuleState")) RuleState.Clear();
            }  

            using var ruleStateResultsTable = ImRaii.Table("ruleStateResultsTable", 4, ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY | ImGuiTableFlags.Resizable, new(availableRegion.X, availableRegion.Y / 2));
            if (ruleStateResultsTable)
            {
                ImGui.TableSetupColumn($"##ruleStateResultActions", ImGuiTableColumnFlags.None, 1);
                ImGui.TableSetupColumn($"Mod Directory##ruleStateResultDirectories", ImGuiTableColumnFlags.None, 4);
                ImGui.TableSetupColumn($"Path##ruleStateResultPaths", ImGuiTableColumnFlags.None, 4);
                ImGui.TableSetupColumn($"New Path##ruleStateResultNewPaths", ImGuiTableColumnFlags.None, 4);
                ImGui.TableSetupScrollFreeze(0, 2);
                ImGui.TableHeadersRow();

                ImGui.TableNextColumn();

                var clearButtonWidth = ImGui.CalcTextSize("NNNN").X;
                if (ImGui.TableNextColumn())
                {
                    var directoryFilter = RuleState.DirectoryFilter;
                    ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - clearButtonWidth);
                    if (ImGui.InputTextWithHint("##ruleResultDirectoryFilter", Texts.FilterHint, ref directoryFilter, ushort.MaxValue)) RuleState.DirectoryFilter = directoryFilter;
                    ImGui.SameLine();
                    if (ImGui.Button("X###clearRuleResultDirectoryFilter")) RuleState.DirectoryFilter = string.Empty;
                }

                if (ImGui.TableNextColumn())
                {
                    var pathFilter = RuleState.PathFilter;
                    ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - clearButtonWidth);
                    if (ImGui.InputTextWithHint("##ruleResultPathFilter", Texts.FilterHint, ref pathFilter)) RuleState.PathFilter = pathFilter;
                    ImGui.SameLine();
                    if (ImGui.Button("X##clearRuleResultPathFilter")) RuleState.PathFilter = string.Empty;
                }

                if (ImGui.TableNextColumn())
                {
                    var newPathFilter = RuleState.NewPathFilter;
                    ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - clearButtonWidth);
                    if (ImGui.InputTextWithHint("##ruleResultNewPathFilter", Texts.FilterHint, ref newPathFilter)) RuleState.NewPathFilter = newPathFilter;
                    ImGui.SameLine();
                    if (ImGui.Button("X##clearRuleResultNewPathFilter")) RuleState.NewPathFilter = string.Empty;
                }

                var showedRuleResults = RuleState.GetShowedResults<RuleResult, IShowableRuleResultState>().Order();

                using var clipperResource = new ImRaiiListClipper(showedRuleResults.Count(), GetItemHeight());
                var clipper = clipperResource.Value;

                while (clipper.Step())
                {
                    for (var i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
                    {
                        var ruleResult = showedRuleResults.ElementAt(i);

                        if (ImGui.TableNextColumn())
                        {
                            if (ruleResult is ISelectableResult selectableResult)
                            {
                                var isSelected = selectableResult.Selected;
                                if (ImGui.Checkbox($"###selectRuleResult{selectableResult.GetHashCode()}", ref isSelected)) selectableResult.Selected = isSelected;
                            } 
                            else
                            {
                                DrawInvisibleCheckbox();
                            }
                        }

                        if (ImGui.TableNextColumn())
                        {
                            ImGui.Text(ruleResult.Directory);
                            if (ImGui.IsItemHovered()) ImGui.SetTooltip(Inspect(ruleResult.Directory));
                        }

                        using var _ = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudGrey3, ruleResult is RuleSamePathResult);

                        if (ImGui.TableNextColumn())
                        {
                            ImGui.Text(ruleResult.Path);
                            if (ImGui.IsItemHovered()) ImGui.SetTooltip(Inspect(ruleResult.Path));
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
                                case IError error:
                                    DrawError(error);
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

        using (ImRaii.PushColor(ImGuiCol.ChildBg, CustomColors.LightBlack))
        {
            using var _ = ImRaii.PushStyle(ImGuiStyleVar.WindowPadding, new Vector2(5, 5));
            using var topRightPanel = ImRaii.Child("mainTopRightPanel", new(rightRegion.X, rightRegion.Y / 3), false, ImGuiWindowFlags.AlwaysUseWindowPadding);
            foreach (var selectedModDirectory in SelectedModDirectories.OrderBy(d => d, StringComparer.OrdinalIgnoreCase))
            {
                using var modInfoNode = ImRaii.TreeNode($"{selectedModDirectory}##modInfo{selectedModDirectory.GetHashCode()}");

                using var __ = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudWhite2);
                if (modInfoNode.Success && ModInterop.TryGetModInfo(selectedModDirectory, out var modInfo)) DrawObjectTree(modInfo);
            }
        }

        using var bottomRightPanel = ImRaii.Child("mainBottomRightPanel");

        ImGui.SameLine();
        using (ImRaii.Color? _ = ImRaii.PushColor(ImGuiCol.Button, CustomColors.LightBlue), __ = ImRaii.PushColor(ImGuiCol.Text, CustomColors.Black))
        {
            if (ImGui.Button("Evaluate##evaluateEvaluationState")) EvaluationState.Evaluate(SelectedModDirectories);
        }   
        ImGuiComponents.HelpMarker("Scriban syntax, check documentation for usage");
        var orderedRules = Config.Rules.OrderByDescending(r => r.Priority);
        var selectedRuleItemIndex = 0;
        ImGui.SameLine();
        if (ImGui.Combo("##loadEvaluationStateRule", ref selectedRuleItemIndex, orderedRules.Select(r => $"{r.Path} ({r.Priority})").Prepend("Load Rule...").ToArray()) && selectedRuleItemIndex > 0) EvaluationState.Load(orderedRules.ElementAt(selectedRuleItemIndex - 1));

        ImGui.SameLine(rightRegion.X - 70);
        using (ImRaii.Disabled(!EvaluationState.GetResults().Any()))
        {
            using (ImRaii.PushColor(ImGuiCol.Button, ImGuiColors.DalamudRed))
            {
                if (ImGui.Button("Clear All##clearEvaluationState")) EvaluationState.Clear();
            }
        }
            
        var bottomWidgetSize = new Vector2(rightRegion.X, rightRegion.Y / 8);
        using (ImRaii.PushColor(ImGuiCol.FrameBg, CustomColors.LightBlack))
        {
            var expression = EvaluationState.Expression;
            if (ImGui.InputTextMultiline("##evaluationStateExpression", ref expression, ushort.MaxValue, bottomWidgetSize)) EvaluationState.Expression = expression;

            var template = EvaluationState.Template;
            if (ImGui.InputTextMultiline("##evaluationStateTemplate", ref template, ushort.MaxValue, bottomWidgetSize)) EvaluationState.Template = template;
        }

        if (EvaluationState.GetResults().Any())
        {
            using var evaluationStateResultsTable = ImRaii.Table("evaluationStateResultsTable", 3, ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY | ImGuiTableFlags.Resizable, ImGui.GetContentRegionAvail());

            if (!evaluationStateResultsTable) return;
            
            ImGui.TableSetupColumn($"Mod Directory##evaluationStateResultDirectories", ImGuiTableColumnFlags.None, 1);
            ImGui.TableSetupColumn($"Expression Result##evaluationStateExpressionValues", ImGuiTableColumnFlags.None, 3);
            ImGui.TableSetupColumn($"Template Result##evaluationStateTemplateValues", ImGuiTableColumnFlags.None, 3);
            ImGui.TableSetupScrollFreeze(0, 2);
            ImGui.TableHeadersRow();

            var buttonWidth = ImGui.CalcTextSize("NNNN").X;
            if (ImGui.TableNextColumn())
            {
                var directoryFilter = EvaluationState.DirectoryFilter;
                ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - buttonWidth);
                if (ImGui.InputTextWithHint("##evaluationStateDirectoryFilter", Texts.FilterHint, ref directoryFilter, ushort.MaxValue)) EvaluationState.DirectoryFilter = directoryFilter;
                ImGui.SameLine();
                if (ImGui.Button("X##clearEvaluationStateDirectoryFilter")) EvaluationState.DirectoryFilter = string.Empty;
            }

            if (ImGui.TableNextColumn())
            {
                var expressionFilter = EvaluationState.ExpressionFilter;
                ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - buttonWidth);
                if (ImGui.InputTextWithHint("##evaluationStateExpressionFilter", Texts.FilterHint, ref expressionFilter)) EvaluationState.ExpressionFilter = expressionFilter;
                ImGui.SameLine();
                if (ImGui.Button("X##clearEvaluationStateExpressionFilter")) EvaluationState.ExpressionFilter = string.Empty;
            }

            if (ImGui.TableNextColumn())
            {
                var templateFilter = EvaluationState.TemplateFilter;
                ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - buttonWidth);
                if (ImGui.InputTextWithHint("##evaluationStateTemplateFilter", Texts.FilterHint, ref templateFilter)) EvaluationState.TemplateFilter = templateFilter;
                ImGui.SameLine();
                if (ImGui.Button("X##clearEvaluationStateTemplateFilter")) EvaluationState.TemplateFilter = string.Empty;
            }

            var showedEvaluationResults = EvaluationState.GetShowedResults<EvaluationResult, IShowableEvaluationResultState>().Order();

            using var clipperResource = new ImRaiiListClipper(showedEvaluationResults.Count(), ImGui.GetTextLineHeightWithSpacing());
            var clipper = clipperResource.Value;

            while (clipper.Step())
            {
                for (var i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
                {
                    var evaluationResult = showedEvaluationResults.ElementAt(i);

                    if (ImGui.TableNextColumn())
                    {
                        ImGui.Text(evaluationResult.Directory);
                        if (ImGui.IsItemHovered()) ImGui.SetTooltip(Inspect(evaluationResult.Directory));
                    }

                    if (ImGui.TableNextColumn())
                    {
                        if (evaluationResult.ExpressionError == null)
                        {
                            ImGui.Text(evaluationResult.ExpressionValue);
                            if (ImGui.IsItemHovered()) ImGui.SetTooltip(Inspect(evaluationResult.ExpressionValue));
                        } 
                        else
                        {
                            DrawError(evaluationResult.ExpressionError);
                        } 
                    }

                    if (ImGui.TableNextColumn())
                    {
                        if (evaluationResult.TemplateError == null)
                        {
                            ImGui.Text(evaluationResult.TemplateValue);
                            if (ImGui.IsItemHovered()) ImGui.SetTooltip(Inspect(evaluationResult.TemplateValue));
                        }
                        else
                        {
                            DrawError(evaluationResult.TemplateError);
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

            DrawMemberTree(member, memberValue, $"objectTree{value.GetHashCode()}");
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
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(Inspect(rulePathResult.NewPath));
    }

    private void DrawResult(RuleSamePathResult ruleSamePathResult)
    {
        ImGui.Text(ruleSamePathResult.Path);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(Inspect(ruleSamePathResult.Path));
    }

    private static void DrawError(IError error)
    {
        using var _ = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudRed);
        ImGui.Text(error.Message);
        if (error.InnerMessage != null && ImGui.IsItemHovered()) ImGui.SetTooltip(error.InnerMessage);
    }

    private static float GetItemHeight() => ImGui.GetTextLineHeightWithSpacing() + (2 * ImGui.GetStyle().FramePadding.Y);

    private static void DrawInvisibleCheckbox()
    {
        using var _ = ImRaii.Disabled();
        using var __ = ImRaii.PushColor(ImGuiCol.FrameBg, 0);
        var ___ = false;
        ImGui.Checkbox(string.Empty, ref ___);
    }

    private string Inspect(object? value) => ViewTemplateContext.ObjectToString(value, true);

}
