using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Lumina.Text.ReadOnly;
using ModOrganizer.Mods;
using ModOrganizer.Scriban;
using ModOrganizer.Utils;
using Scriban;
using Scriban.Helpers;
using Scriban.Parsing;
using Scriban.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace ModOrganizer.Windows;

public class MainWindow : Window, IDisposable
{
    private static readonly Vector4 LIGHT_BLUE = new(0.753f, 0.941f, 1, 1);
    private static readonly Vector4 BLACK = new(0.2f, 0.2f, 0.2f, 0.5f);

    private ModInterop ModInterop { get; init; }
    private ModVirtualFileSystem ModVirtualFileSystem { get; init; }
    private IPluginLog PluginLog { get; init; }
    private SourceSpan SourceSpan { get; init; } = new();

    private string Filter { get; set; } = string.Empty;
    private HashSet<string> SelectedModDirectories { get; set; } = [];

    private string Expression { get; set; } = string.Empty;

    private string EvalationModDirectoryFilter { get; set; } = string.Empty;
    private string EvalationEvaluationResultFilter { get; set; } = string.Empty;
    private Dictionary<string, object> EvaluationResults { get; set; } = [];
    private Task EvaluationTask { get; set; } = Task.CompletedTask;

    private TemplateContext ViewTemplateContext { get; init; } = new() { MemberRenamer = MemberRenamer.Rename };
    
    public MainWindow(ModInterop modInterop, ModVirtualFileSystem modVirtualFileSystem, IPluginLog pluginLog) : base("ModOrganizer - Main##mainWindow")
    {
        SizeConstraints = new()
        {
            MinimumSize = new(375, 330),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };

        ModInterop = modInterop;
        ModVirtualFileSystem = modVirtualFileSystem;
        PluginLog = pluginLog;

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

    private void ToggleFolderSelection(ModVirtualFolder folder)
    {
        var modDirectories = folder.GetNestedFiles().Select(f => f.Directory);
        if (SelectedModDirectories.Overlaps(modDirectories))
        {
            SelectedModDirectories.ExceptWith(modDirectories);
        }
        else
        {
            SelectedModDirectories.UnionWith(modDirectories);
        }
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

            // TODO: fix
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
                if (ImGui.InputTextWithHint("###filter", "Filter...", ref filter))
                {
                    Filter = filter;
                }
                ImGui.SameLine();
                if (ImGui.Button("X###clearFilter"))
                {
                    Filter = string.Empty;
                }
            }

            var leftBottomRegion = ImGui.GetContentRegionAvail();
            using var leftBottomPanel = ImRaii.Child("bottomLeftPanel", leftBottomRegion);
            if (ModVirtualFileSystem.GetRootFolder().TrySearch(filter, out var filteredFolder))
            {
                DrawVirtualFolderTree(filteredFolder);
            }
        }

        ImGui.SameLine();

        using var rightPanel = ImRaii.Child("rightPanel", new(fullRegion.X - leftPanelWidth, fullRegion.Y));

        var rightRegion = ImGui.GetContentRegionAvail();
        var topRightHeaderHeight = ImGui.GetFrameHeightWithSpacing() * 2;
        var bottomRightHeight = ImGui.GetFrameHeightWithSpacing() * 15;

        var topRightPanelHeight = rightRegion.Y - bottomRightHeight - topRightHeaderHeight;

        var topRightPanelSize = new Vector2(rightRegion.X, topRightPanelHeight);

        if (SelectedModDirectories.Count == 0)
        {
            ImGui.Text("No mod selected");
            ImGuiComponents.HelpMarker("Click on the left panel to select one and hold <L-CTRL> or <L-SHIFT> for multi-selection");
        }
        else
        {
            var topRightHeaderOpened = ImGui.CollapsingHeader("Inspector##inspectorHeader", ImGuiTreeNodeFlags.DefaultOpen);
            if (topRightHeaderOpened)
            {

                using (ImRaii.PushColor(ImGuiCol.ChildBg, BLACK))
                {
                    using var topRightPanel = ImRaii.Child("topRightPanel", topRightPanelSize);
                    using var _ = ImRaii.PushIndent();
                    foreach (var selectedModDirectory in SelectedModDirectories)
                    {
                        using var modInfoNode = ImRaii.TreeNode($"{selectedModDirectory}##modInfo{selectedModDirectory.GetHashCode()}");

                        using var __ = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudWhite2);
                        if (modInfoNode.Success && ModInterop.TryGetModInfo(selectedModDirectory, out var modInfo)) DrawObjectTree(modInfo);
                    }
                }
            }

            using var bottomRightPanel = ImRaii.Child("bottomRightPanel", new(rightRegion.X, topRightHeaderOpened ? bottomRightHeight : rightRegion.Y - topRightHeaderHeight));

            var bottomRightButtonsWidth = ImGui.CalcTextSize("NNNNNNNNNNNNNNNNN").X;
            var bottomWidgetSize = new Vector2(rightRegion.X - bottomRightButtonsWidth, ImGui.GetFrameHeightWithSpacing() * 4);

            var expression = Expression;
            using (ImRaii.PushColor(ImGuiCol.FrameBg, BLACK))
            {
                if (ImGui.InputTextMultiline("##expressionInput", ref expression, ushort.MaxValue, bottomWidgetSize))
                {
                    Expression = expression;
                }

            }
            
            ImGui.SameLine();
            if (ImGui.Button("Evaluate##evaluateButton"))
            {
                Evaluate();
            }

            ImGui.SameLine();
            if (ImGui.Button("Clear##clearButton"))
            {
                Expression = string.Empty;
                EvalationModDirectoryFilter = string.Empty;
                EvalationEvaluationResultFilter = string.Empty;
                EvaluationResults = [];
            }

            if (EvaluationResults.Count > 0)
            {
                using var table = ImRaii.Table("evaluationResultsTable", 2, ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY | ImGuiTableFlags.Resizable, ImGui.GetContentRegionAvail());

                if (table)
                {
                    // Add column search

                    // Add clipping ImGui.ImGuiListClipper();
                    ImGui.TableSetupColumn($"Mod directory###directoryName", ImGuiTableColumnFlags.None, 1);
                    ImGui.TableSetupColumn($"Evaluation result###result", ImGuiTableColumnFlags.None, 6);
                    ImGui.TableSetupScrollFreeze(0, 2);
                    ImGui.TableHeadersRow();

                    if (ImGui.TableNextColumn())
                    {
                        var filter = EvalationModDirectoryFilter;
                        if (ImGui.InputText("###evalationModDirectoryFilter", ref filter)) EvalationModDirectoryFilter = filter;
                        if (ImGui.Button("X###clearEvalationModDirectoryFilter")) EvalationModDirectoryFilter = string.Empty;
                    }

                    if (ImGui.TableNextColumn())
                    {
                        var filter = EvalationEvaluationResultFilter;
                        if (ImGui.InputText("###evalationModDirectoryFilter", ref filter)) EvalationEvaluationResultFilter = filter;
                        if (ImGui.Button("X###clearEvalationModDirectoryFilter")) EvalationEvaluationResultFilter = string.Empty;
                    }

                    foreach (var evaluationResult in EvaluationResults.Where(e => TokenMatcher.Matches(EvalationModDirectoryFilter, e.Key) || TokenMatcher.Matches(EvalationModDirectoryFilter, e.Value as string)).OrderBy(r => r.Key, StringComparer.OrdinalIgnoreCase))
                    {
                        if (ImGui.TableNextColumn())
                        {
                            ImGui.Text(evaluationResult.Key);
                            if (ImGui.IsItemHovered()) ImGui.SetTooltip(evaluationResult.Key);
                        }

                        if (ImGui.TableNextColumn())
                        {
                            if (evaluationResult.Value is Exception e)
                            {
                                using var _ = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudRed);
                                ImGui.Text(e.Message);
                                if (ImGui.IsItemHovered() && e.InnerException != null) ImGui.SetTooltip(e.InnerException.Message);
                                continue;
                            }

                            if (evaluationResult.Value is string value)
                            {
                                ImGui.Text(value);
                                if (ImGui.IsItemHovered()) ImGui.SetTooltip(value);
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
        foreach (var member in accessor.GetMembers(ViewTemplateContext, SourceSpan, value))
        {
            if (!accessor.TryGetValue(ViewTemplateContext, SourceSpan, value, member, out var memberValue)) continue;

            DrawMemberTree(member, memberValue, $"inspect{value.GetHashCode()}");
        }
    }

    private void DrawMemberTree(string name, object? value, string baseId)
    {
        if (value == null)
        {
            using var _ = ImRaii.TreeNode($"{name}: null###{baseId}{name}", ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Bullet);
            return;
        }

        var memberType = value.GetType();
        var isEmptyList = value is IList l && l.Count == 0;
        var isEmptyDict = value is IDictionary d && d.Count == 0;

        var isPrintable = (memberType.IsPrimitive || memberType.IsEnum || typeof(string).IsAssignableFrom(memberType) || typeof(ReadOnlySeString).IsAssignableFrom(memberType) || isEmptyList || isEmptyDict);
        var isLeaf = memberType.IsPrimitive || isEmptyList || isEmptyDict;

        using var treeNode = ImRaii.TreeNode($"{name}: {(isPrintable ? ViewTemplateContext.ObjectToString(value) : "")} ({memberType.ScriptPrettyName()})###inspect{value.GetHashCode()}{name}", isLeaf ? ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Bullet : ImGuiTreeNodeFlags.None);
        if (!treeNode) return;

        if (value is IList nestedValues)
        {
            for (var i = 0; i < nestedValues.Count; i++) DrawMemberTree($"[{i}]", nestedValues[i], baseId);
        } 
        else
        {
            DrawObjectTree(value);
        }   
    }

    // Make generic to support evaluating rules
    private void Evaluate()
    {
        EvaluationTask = EvaluationTask.ContinueWith(_ =>
        {
            // Clear while waiting for new results
            EvaluationResults = [];

            EvaluationResults = SelectedModDirectories.ToDictionary(d => d, modDirectory =>
            {
                if (!ModInterop.TryGetModInfo(modDirectory, out var modInfo)) return new ArgumentException("Failed to retrieve mod data");

                var templateContext = new TemplateContext() { MemberRenamer = MemberRenamer.Rename };

                var scriptObject = new ScriptObject();
                scriptObject.Import(modInfo);
                templateContext.PushGlobal(scriptObject);

                try
                {
                    var result = Template.Evaluate(Expression, templateContext);
                    return (object)templateContext.ObjectToString(result)!;
                }
                catch (Exception e)
                {
                    PluginLog.Warning($"Failed to evaluate expression [{Expression}] for mod [{modDirectory}]:\n\t{e.Message}");
                    return new ArgumentException("Failed to evaluate", e);
                }
            });
        });
    }
}
