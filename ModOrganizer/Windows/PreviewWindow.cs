using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ModOrganizer.Utils;
using ModOrganizer.Virtuals;
using ModOrganizer.Windows.States;
using ModOrganizer.Windows.States.Results.Rules;
using System;
using System.Linq;

namespace ModOrganizer.Windows;

public class PreviewWindow : Window, IDisposable
{
    private RuleResultFileSystem RuleResultFileSystem { get; init; }

    private string Filter { get; set; } = string.Empty;

    public bool ShowUnselected { get; set; } = false;

    public PreviewWindow(RuleEvaluationState ruleEvaluationState) : base("ModOrganizer - Preview##previewWindow")
    {
        SizeConstraints = new()
        {
            MinimumSize = new(375, 330),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };

        RuleResultFileSystem = new(ruleEvaluationState);
    }

    public void Dispose() => RuleResultFileSystem.Dispose();

    public override void Draw()
    {
        if (!RuleResultFileSystem.HasRulePathResults())
        {
            ImGui.Text("No rule path results to preview");
            return;
        }

        var filter = Filter;
        if (ImGui.InputTextWithHint("##resultFilter", Constants.FILTER_HINT, ref filter)) Filter = filter;
        ImGui.SameLine();
        if (ImGui.Button("X##clearResultFilter")) Filter = string.Empty;

        ImGui.SameLine(50 - ImGui.GetWindowWidth());
        var showUnselected = ShowUnselected;
        if (ImGui.Checkbox("Show Unselected##showUnselectedResults", ref showUnselected)) ShowUnselected = showUnselected;

        if (RuleResultFileSystem.GetRootFolder().TrySearch(filter, out var filteredFolder)) DrawVirtualFolderTree(filteredFolder);
    }

    private void DrawVirtualFolderTree(VirtualFolder folder)
    {
        var orderedSubfolders = folder.Folders.OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase);
        foreach (var subfolder in orderedSubfolders)
        {
            using var _ = ImRaii.PushColor(ImGuiCol.Text, Constants.LIGHT_BLUE);
            using var treeNode = ImRaii.TreeNode($"{subfolder.Name}###resultVirtualFolder{subfolder.GetHashCode()}");
            if (treeNode) DrawVirtualFolderTree(subfolder);
        }

        var orderedFiles = folder.Files.OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase);
        foreach (var file in orderedFiles)
        {
            if (!RuleResultFileSystem.TryGetRulePathResult(file.Directory, out var rulePathResult)) continue;

            var isSelected = rulePathResult.IsSelected;
            if (!isSelected && !ShowUnselected) continue;

            using var _ = ImRaii.PushColor(ImGuiCol.Text, isSelected ? ImGuiColors.DalamudWhite : ImGuiColors.DalamudGrey3);
            using var __ = ImRaii.TreeNode($"{file.Name}###resultVirtualFile{file.GetHashCode()}", ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Bullet);

            if (ImGui.IsItemHovered()) ImGui.SetTooltip($"Current [{rulePathResult.CurrentPath}]");
        }
    }
}
