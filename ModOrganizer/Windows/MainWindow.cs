using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Lumina.Text.ReadOnly;
using ModOrganizer.Configs;
using ModOrganizer.Mods;
using ModOrganizer.Shared;
using ModOrganizer.Virtuals;
using ModOrganizer.Windows.Configs;
using ModOrganizer.Windows.Results;
using ModOrganizer.Windows.Results.Rules;
using ModOrganizer.Windows.Results.Selectables;
using ModOrganizer.Windows.Results.Showables;
using ModOrganizer.Windows.Togglers;
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

public class MainWindow : Window
{
    private IConfig Config { get; init; }
    private EvaluationResultState EvaluationResultState { get; init; }
    private IModInterop ModInterop { get; init; }
    private IModFileSystem ModFileSystem { get; init; }
    private RuleResultState RuleResultState { get; init; }
    private IWindowToggler WindowToggler { get; init; }

    private string Filter { get; set; } = string.Empty;
    private HashSet<string> SelectedModDirectories { get; set; } = [];

    private TemplateContext ViewTemplateContext { get; init; } = new() { MemberRenamer = MemberRenamer.Rename };
    private SourceSpan ViewSourceSpan { get; init; } = new();

    public MainWindow(IConfig config, IModInterop modInterop, EvaluationResultState evaluationResultState, IModFileSystem modFileSystem, RuleResultState ruleResultState, IWindowToggler windowToggler) : base("ModOrganizer - Main##mainWindow")
    {
        Config = config;
        EvaluationResultState = evaluationResultState;
        ModInterop = modInterop;
        ModFileSystem = modFileSystem;
        RuleResultState = ruleResultState;
        WindowToggler = windowToggler;

        SizeConstraints = new()
        {
            MinimumSize = new(1200, 850),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };

        TitleBarButtons = [
            new()
            {
                Icon = FontAwesomeIcon.Cog, 
                ShowTooltip = () => ImGui.SetTooltip("Toggle config window"), 
                Click = _ => WindowToggler.Toggle<ConfigWindow>() 
            },
            new() 
            {
                Icon = FontAwesomeIcon.Database,
                ShowTooltip = () => ImGui.SetTooltip("Toggle backup window"),
                Click = _ => WindowToggler.Toggle<BackupWindow>()
            },
            new() 
            {
                Icon = FontAwesomeIcon.Eye,
                ShowTooltip = () => ImGui.SetTooltip("Toggle preview window"),
                Click = _ => WindowToggler.Toggle<PreviewWindow>()
            }
        ];

        ModInterop.OnModDeleted += OnModDeleted;
        ModInterop.OnModMoved += OnModMoved;
    }

    public void Dispose() 
    {
        ModInterop.OnModDeleted -= OnModDeleted;
        ModInterop.OnModMoved -= OnModMoved;

        ModInterop.ToggleFsWatchers(false);
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
        SelectedModDirectories = [.. SelectedModDirectories.Overlaps(modDirectories) ? 
            SelectedModDirectories.Except(modDirectories) : SelectedModDirectories.Union(modDirectories)];
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
            using var __ = ImRaii.TreeNode($"{file.Name}###modVirtualFile{hash}", ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Bullet | 
                (SelectedModDirectories.Contains(file.Directory) ? ImGuiTreeNodeFlags.Selected : ImGuiTreeNodeFlags.None));

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
        var hasCompletedTask = RuleResultState.HasCompletedTask();

        if (SelectedModDirectories.Count > 1)
        {
            using (ImRaii.Disabled(!hasCompletedTask))
            {
                using ImRaii.Color? _ = ImRaii.PushColor(ImGuiCol.Button, CustomColors.LightBlue), __ = ImRaii.PushColor(ImGuiCol.Text, CustomColors.Black);
                if (ImGui.Button($"Preview All##evaluateModDirectories")) RuleResultState.Preview(SelectedModDirectories);
            } 

            ImGui.SameLine();
            ImGui.Text($"Selection Count: {SelectedModDirectories.Count}");
        }

        var availableRegion = ImGui.GetContentRegionAvail();
        var hasResults = RuleResultState.GetResults().Any();

        using (var selectedModsDirectoriesTable = ImRaii.Table("selectedModsDirectoriesTable", 2, ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY | 
            ImGuiTableFlags.Resizable, hasResults ? new(availableRegion.X, (availableRegion.Y / 2) - (2 * ImGui.GetTextLineHeightWithSpacing())) : availableRegion))
        {
            if (selectedModsDirectoriesTable)
            {
                ImGui.TableSetupColumn($"Mod Directory##selectedModDirectoryNames", ImGuiTableColumnFlags.None, 6);
                ImGui.TableSetupColumn($"Actions##selectedModDirectoryActions", ImGuiTableColumnFlags.None, 1);
                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableHeadersRow();

                var orderedModDirectories = SelectedModDirectories.OrderBy(d => d, StringComparer.OrdinalIgnoreCase).ToList();

                using var imRaiiListClipper = new ImRaiiListClipper(orderedModDirectories.Count, GetItemHeight());
                var imGuiListClipper = imRaiiListClipper.Value;

                while (imGuiListClipper.Step())
                {
                    for (var i = imGuiListClipper.DisplayStart; i < imGuiListClipper.DisplayEnd; i++)
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

                            using var _ = ImRaii.Disabled(!hasCompletedTask);
                            if (ImGui.Button($"Preview###evaluateModDirectory{i}")) RuleResultState.Preview([modDirectory]);
                        }
                    }
                }
            }
        }

        if (!hasResults) return;

        var isConfirmPressed = ImGui.GetIO().KeyCtrl;
        var selectedCount = RuleResultState.GetSelectedResults().Count();

        using (ImRaii.Disabled(!hasCompletedTask || !isConfirmPressed))
        {
            using ImRaii.Color? _ = ImRaii.PushColor(ImGuiCol.Button, ImGuiColors.ParsedGreen), __ = ImRaii.PushColor(ImGuiCol.Text, CustomColors.Black);
            if (ImGui.Button($"Apply Selected ({selectedCount})##applyRuleState")) RuleResultState.Apply();
        }
        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled)) ImGui.SetTooltip(Texts.ConfirmHint);

        using (ImRaii.Disabled(selectedCount == 0))
        {
            ImGui.SameLine();
            if (ImGui.Button("Clear Selection##clearRuleStateSelection")) RuleResultState.ClearResultSelection();
        }

        ImGui.SameLine();
        using (ImRaii.Disabled(!RuleResultState.GetSelectableResults().Any()))
        {
            if (ImGui.Button("Invert Selection##invertRuleStateSelection")) RuleResultState.InvertResultSelection();
        }

        ImGui.SameLine();
        var showErrors = RuleResultState.ShowErrors;
        if (ImGui.Checkbox("Show Errors##showRuleStateErrors", ref showErrors)) RuleResultState.ShowErrors = showErrors;
        ImGui.SameLine();

        var showSamePaths = RuleResultState.ShowSamePaths;
        if (ImGui.Checkbox("Show Same Paths##showRuleStateSameRule", ref showSamePaths)) RuleResultState.ShowSamePaths = showSamePaths;

        ImGui.SameLine(availableRegion.X - 300);
        if (ImGui.Button("Show Backups##toggleBackupWindow")) WindowToggler.Toggle<BackupWindow>();

        ImGui.SameLine();
        if (ImGui.Button("Preview Tree##togglePreviewWindow")) WindowToggler.Toggle<PreviewWindow>();

        ImGui.SameLine();
        using (ImRaii.PushColor(ImGuiCol.Button, ImGuiColors.DalamudRed))
        {
            if (ImGui.Button("Clear All##clearRuleState")) RuleResultState.Clear();
        }  

        using var ruleStateResultsTable = ImRaii.Table("ruleStateResultsTable", 4, ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY | ImGuiTableFlags.Resizable, 
            new(availableRegion.X, availableRegion.Y / 2));

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
                var directoryFilter = RuleResultState.DirectoryFilter;
                ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - clearButtonWidth);
                if (ImGui.InputTextWithHint("##ruleResultDirectoryFilter", Texts.FilterHint, ref directoryFilter, ushort.MaxValue)) RuleResultState.DirectoryFilter = directoryFilter;
                ImGui.SameLine();
                if (ImGui.Button("X###clearRuleResultDirectoryFilter")) RuleResultState.DirectoryFilter = string.Empty;
            }

            if (ImGui.TableNextColumn())
            {
                var pathFilter = RuleResultState.PathFilter;
                ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - clearButtonWidth);
                if (ImGui.InputTextWithHint("##ruleResultPathFilter", Texts.FilterHint, ref pathFilter)) RuleResultState.PathFilter = pathFilter;
                ImGui.SameLine();
                if (ImGui.Button("X##clearRuleResultPathFilter")) RuleResultState.PathFilter = string.Empty;
            }

            if (ImGui.TableNextColumn())
            {
                var newPathFilter = RuleResultState.NewPathFilter;
                ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - clearButtonWidth);
                if (ImGui.InputTextWithHint("##ruleResultNewPathFilter", Texts.FilterHint, ref newPathFilter)) RuleResultState.NewPathFilter = newPathFilter;
                ImGui.SameLine();
                if (ImGui.Button("X##clearRuleResultNewPathFilter")) RuleResultState.NewPathFilter = string.Empty;
            }

            var showedRuleResults = RuleResultState.GetShowedRuleResults().Order().ToList();

            using var imRaiiListClipper = new ImRaiiListClipper(showedRuleResults.Count, GetItemHeight());
            var imGuiListClipper = imRaiiListClipper.Value;

            while (imGuiListClipper.Step())
            {
                for (var i = imGuiListClipper.DisplayStart; i < imGuiListClipper.DisplayEnd; i++)
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

        using (ImRaii.Disabled(!EvaluationResultState.HasCompletedTask()))
        {
            using ImRaii.Color? _ = ImRaii.PushColor(ImGuiCol.Button, CustomColors.LightBlue), __ = ImRaii.PushColor(ImGuiCol.Text, CustomColors.Black);
            if (ImGui.Button("Evaluate##evaluateEvaluationState")) EvaluationResultState.Evaluate(SelectedModDirectories);
        }   
        ImGuiComponents.HelpMarker("Scriban syntax, check online documentation for usage");

        var orderedRules = Config.Rules.OrderByDescending(r => r.Priority).ToList();
        var selectedRuleItemIndex = 0;
        ImGui.SameLine();
        if (ImGui.Combo("##loadEvaluationStateRule", ref selectedRuleItemIndex, orderedRules.Select(r => $"{r.Path} ({r.Priority})").Prepend("Load Rule...").ToArray()) 
            && selectedRuleItemIndex > 0) EvaluationResultState.Load(orderedRules.ElementAt(selectedRuleItemIndex - 1));

        ImGui.SameLine(rightRegion.X - 70);

        var hasResults = EvaluationResultState.GetResults().Any();
        using (ImRaii.Disabled(!hasResults))
        {
            using (ImRaii.PushColor(ImGuiCol.Button, ImGuiColors.DalamudRed))
            {
                if (ImGui.Button("Clear All##clearEvaluationState")) EvaluationResultState.Clear();
            }
        }
            
        var bottomWidgetSize = new Vector2(rightRegion.X, rightRegion.Y / 8);
        using (ImRaii.PushColor(ImGuiCol.FrameBg, CustomColors.LightBlack))
        {
            var expression = EvaluationResultState.Expression;
            if (ImGui.InputTextMultiline("##evaluationStateExpression", ref expression, ushort.MaxValue, bottomWidgetSize)) EvaluationResultState.Expression = expression;

            var template = EvaluationResultState.Template;
            if (ImGui.InputTextMultiline("##evaluationStateTemplate", ref template, ushort.MaxValue, bottomWidgetSize)) EvaluationResultState.Template = template;
        }

        if (!hasResults) return;

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
            var directoryFilter = EvaluationResultState.DirectoryFilter;
            ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - buttonWidth);
            if (ImGui.InputTextWithHint("##evaluationStateDirectoryFilter", Texts.FilterHint, ref directoryFilter, ushort.MaxValue)) EvaluationResultState.DirectoryFilter = directoryFilter;
            ImGui.SameLine();
            if (ImGui.Button("X##clearEvaluationStateDirectoryFilter")) EvaluationResultState.DirectoryFilter = string.Empty;
        }

        if (ImGui.TableNextColumn())
        {
            var expressionFilter = EvaluationResultState.ExpressionFilter;
            ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - buttonWidth);
            if (ImGui.InputTextWithHint("##evaluationStateExpressionFilter", Texts.FilterHint, ref expressionFilter)) EvaluationResultState.ExpressionFilter = expressionFilter;
            ImGui.SameLine();
            if (ImGui.Button("X##clearEvaluationStateExpressionFilter")) EvaluationResultState.ExpressionFilter = string.Empty;
        }

        if (ImGui.TableNextColumn())
        {
            var templateFilter = EvaluationResultState.TemplateFilter;
            ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - buttonWidth);
            if (ImGui.InputTextWithHint("##evaluationStateTemplateFilter", Texts.FilterHint, ref templateFilter)) EvaluationResultState.TemplateFilter = templateFilter;
            ImGui.SameLine();
            if (ImGui.Button("X##clearEvaluationStateTemplateFilter")) EvaluationResultState.TemplateFilter = string.Empty;
        }

        var showedEvaluationResults = EvaluationResultState.GetShowedEvaluationResults().Order().ToList();

        using var imRaiiListClipper = new ImRaiiListClipper(showedEvaluationResults.Count, ImGui.GetTextLineHeightWithSpacing());
        var imGuiListClipper = imRaiiListClipper.Value;

        while (imGuiListClipper.Step())
        {
            for (var i = imGuiListClipper.DisplayStart; i < imGuiListClipper.DisplayEnd; i++)
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

        using var treeNode = ImRaii.TreeNode($"{name}: {(isPrintable ? ViewTemplateContext.ObjectToString(value) : string.Empty)} ({valueType.ScriptPrettyName()})###inspect{value.GetHashCode()}{name}", 
            isLeaf ? ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Bullet : ImGuiTreeNodeFlags.None);

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
