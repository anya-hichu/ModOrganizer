using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Lumina.Text.ReadOnly;
using ModOrganizer.Mods;
using ModOrganizer.Rules;
using ModOrganizer.Scriban;
using ModOrganizer.Utils;
using ModOrganizer.Windows.States;
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
    private static readonly Vector4 LIGHT_BLUE = new(0.753f, 0.941f, 1, 1);
    private static readonly Vector4 BLACK = new(0.2f, 0.2f, 0.2f, 0.5f);

    private static readonly string FILTER_HINT = "Filter...";

    private Config Config { get; init; }
    private ModInterop ModInterop { get; init; }
    private ModVirtualFileSystem ModVirtualFileSystem { get; init; }
    private IPluginLog PluginLog { get; init; }

    private RuleEvaluationState RuleEvaluationState { get; init; }
    private EvaluationState EvaluationState { get; init; }

    private string Filter { get; set; } = string.Empty;
    private HashSet<string> SelectedModDirectories { get; set; } = [];

    private TemplateContext ViewTemplateContext { get; init; } = new() { MemberRenamer = MemberRenamer.Rename };
    private SourceSpan ViewSourceSpan { get; init; } = new();

    public MainWindow(Config config, ModInterop modInterop, ModVirtualFileSystem modVirtualFileSystem, IPluginLog pluginLog, RuleEvaluator ruleEvaluator) : base("ModOrganizer - Main##mainWindow")
    {
        SizeConstraints = new()
        {
            MinimumSize = new(375, 330),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };

        Config = config;
        ModInterop = modInterop;
        ModVirtualFileSystem = modVirtualFileSystem;
        PluginLog = pluginLog;

        RuleEvaluationState = new(ModInterop, PluginLog, ruleEvaluator);
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

    private void ToggleFolderSelection(ModVirtualFolder folder)
    {
        var modDirectories = folder.GetNestedFiles().Select(f => f.Directory);
        SelectedModDirectories = [.. SelectedModDirectories.Overlaps(modDirectories) ? SelectedModDirectories.Except(modDirectories) : SelectedModDirectories.Union(modDirectories)];
    }

    private void DrawVirtualFolderTree(ModVirtualFolder folder)
    {
        var orderedSubfolders = folder.Folders.OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase);
        foreach (var subfolder in orderedSubfolders)
        {
            var hash = subfolder.GetHashCode();
            using var _ = ImRaii.PushColor(ImGuiCol.Text, LIGHT_BLUE);
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
                if (ImGui.InputTextWithHint("###filter", FILTER_HINT, ref filter)) Filter = filter;
                ImGui.SameLine();
                if (ImGui.Button("X###clearFilter")) Filter = string.Empty;
            }

            var leftBottomRegion = ImGui.GetContentRegionAvail();
            using var leftBottomPanel = ImRaii.Child("bottomLeftPanel", leftBottomRegion);
            if (ModVirtualFileSystem.GetRootFolder().TrySearch(filter, out var filteredFolder)) DrawVirtualFolderTree(filteredFolder);
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
        if (ImGui.Button("Preview All###evaluateRules")) RuleEvaluationState.PreviewAsync(Config.Rules, SelectedModDirectories);

        var availableRegion = ImGui.GetContentRegionAvail();

        using (var selectedModsTable = ImRaii.Table("selectedModsTable", 2, ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY | ImGuiTableFlags.Resizable, new(availableRegion.X, availableRegion.Y / 2)))
        {
            if (selectedModsTable)
            {
                ImGui.TableSetupColumn($"Mod directory###selectedDirectoryName", ImGuiTableColumnFlags.None, 6);
                ImGui.TableSetupColumn($"Actions###selectedDirectoyActions", ImGuiTableColumnFlags.None, 1);
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
                            if (ImGui.Button($"Unselect###unselectedModDirectory{i}")) SelectedModDirectories.Remove(modDirectory);
                            ImGui.SameLine();
                            if (ImGui.Button($"Preview###evaluateRule{i}")) RuleEvaluationState.PreviewAsync(Config.Rules, [modDirectory]);
                        }
                    }
                }
            }
        }

        if (RuleEvaluationState.Results.Count > 0)
        {
            if (ImGui.Button("Apply All##applyRuleEvaluation")) RuleEvaluationState.ApplyAsync();
            ImGui.SameLine();
            if (ImGui.Button("Clear##clearRuleEvaluationState")) RuleEvaluationState.Clear();

            using var ruleEvaluationResultsTable = ImRaii.Table("ruleEvaluationResultsTable", 2, ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY | ImGuiTableFlags.Resizable, new(availableRegion.X, availableRegion.Y / 2));
            if (ruleEvaluationResultsTable)
            {
                ImGui.TableSetupColumn($"Mod Directory###ruleEvaluationDirectoryName", ImGuiTableColumnFlags.None, 1);
                ImGui.TableSetupColumn($"Path Evaluation Result###pathEvaluationResult", ImGuiTableColumnFlags.None, 6);
                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableHeadersRow();

                var orderedResults = RuleEvaluationState.Results.OrderBy(e => e.Key, StringComparer.OrdinalIgnoreCase).ToList();

                var clipper = ImGui.ImGuiListClipper();
                clipper.Begin(orderedResults.Count, ImGui.GetTextLineHeightWithSpacing());
                while (clipper.Step())
                {
                    for (var i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
                    {
                        var (modDirectory, result) = orderedResults.ElementAt(i);

                        if (ImGui.TableNextColumn())
                        {
                            ImGui.Text(modDirectory);
                            if (ImGui.IsItemHovered()) ImGui.SetTooltip(modDirectory);
                        }

                        if (ImGui.TableNextColumn())
                        {
                            if (result is string value)
                            {
                                ImGui.Text(value);
                                if (ImGui.IsItemHovered()) ImGui.SetTooltip(value);
                                continue;
                            }

                            // null and whatever
                            using var __ = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudGrey3);

                            var formattedValue = result == null ? "null" : result.ToString();
                            ImGui.Text(formattedValue);
                            if (ImGui.IsItemHovered()) ImGui.SetTooltip(formattedValue);
                        }
                    }
                }
            }
        }
    }

    private void DrawInspectorTab()
    {
        var rightRegion = ImGui.GetContentRegionAvail();

        using (ImRaii.PushColor(ImGuiCol.ChildBg, BLACK))
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

        using (ImRaii.PushColor(ImGuiCol.FrameBg, BLACK))
        {
            var expression = EvaluationState.Expression;
            if (ImGui.InputTextMultiline("##evaluationExpression", ref expression, ushort.MaxValue, bottomWidgetSize)) EvaluationState.Expression = expression;
        }

        ImGui.SameLine();
        if (ImGui.Button("Evaluate##evaluateExpression")) EvaluationState.EvaluateAsync(SelectedModDirectories);
        ImGui.SameLine();
        if (ImGui.Button("Clear##clearEvaluationState")) EvaluationState.Clear();

        if (EvaluationState.Results.Count > 0)
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
                    if (ImGui.InputTextWithHint("###evaluationModDirectoryFilter", FILTER_HINT, ref modDirectoryFilter, ushort.MaxValue)) EvaluationState.ModDirectoryFilter = modDirectoryFilter;
                    ImGui.SameLine();
                    if (ImGui.Button("X###clearEvaluationModDirectoryFilter")) EvaluationState.ModDirectoryFilter = string.Empty;
                }

                if (ImGui.TableNextColumn())
                {
                    var resultFilter = EvaluationState.ResultFilter;
                    if (ImGui.InputTextWithHint("###evaluationResultFilter", FILTER_HINT, ref resultFilter)) EvaluationState.ResultFilter = resultFilter;
                    ImGui.SameLine();
                    if (ImGui.Button("X###clearEvaluationResultFilter")) EvaluationState.ResultFilter = string.Empty;
                }

                var filteredResults = EvaluationState.Results.Where(e => TokenMatcher.Matches(EvaluationState.ModDirectoryFilter, e.Key) && TokenMatcher.Matches(EvaluationState.ResultFilter, e.Value as string)).OrderBy(r => r.Key, StringComparer.OrdinalIgnoreCase).ToList();
                
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
                            if (ImGui.IsItemHovered()) ImGui.SetTooltip(modDirectory);
                        }

                        if (ImGui.TableNextColumn())
                        {
                            if (result is Exception e)
                            {
                                using var _ = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudRed);
                                ImGui.Text(e.Message);
                                if (ImGui.IsItemHovered() && e.InnerException != null) ImGui.SetTooltip(e.InnerException.Message);
                                continue;
                            }

                            if (result is string value)
                            {
                                ImGui.Text(value);
                                if (ImGui.IsItemHovered()) ImGui.SetTooltip(value);
                                continue;
                            }

                            // null and whatever
                            using var __ = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudGrey3);

                            var formattedValue = result == null ? "null" : result.ToString();
                            ImGui.Text(formattedValue);
                            if (ImGui.IsItemHovered()) ImGui.SetTooltip(formattedValue);
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

        using var treeNode = ImRaii.TreeNode($"{name}: {(isPrintable ? ViewTemplateContext.ObjectToString(value) : "")} ({valueType.ScriptPrettyName()})###inspect{value.GetHashCode()}{name}", isLeaf ? ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Bullet : ImGuiTreeNodeFlags.None);
        if (!treeNode) return;

        if (value is IList list)
        {
            for (var i = 0; i < list.Count; i++) DrawMemberTree($"[{i}]", list[i], baseId);
            return;
        } 
        
        DrawObjectTree(value);
    }
}
