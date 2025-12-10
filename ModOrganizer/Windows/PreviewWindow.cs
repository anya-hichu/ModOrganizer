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
    private RuleEvaluationState RuleEvaluationState { get; init; }
    private RuleResultFileSystem RuleResultFileSystem { get; init; }

    private string Filter { get; set; } = string.Empty;

    public PreviewWindow(RuleEvaluationState ruleEvaluationState) : base("ModOrganizer - Preview##previewWindow")
    {
        SizeConstraints = new()
        {
            MinimumSize = new(375, 330),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };

        RuleEvaluationState = ruleEvaluationState;
        RuleResultFileSystem = new(RuleEvaluationState);
    }

    public void Dispose() => RuleResultFileSystem.Dispose();

    public override void Draw()
    {
        if (RuleEvaluationState.GetRulePathResultByModDirectory().Count == 0)
        {
            ImGui.Text("No result to display");
            return;
        }

        var filter = Filter;
        if (ImGui.InputTextWithHint("##resultFilter", Constants.FILTER_HINT, ref filter)) Filter = filter;
        ImGui.SameLine();
        if (ImGui.Button("X##clearResultFilter")) Filter = string.Empty;

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
            var isSelected = RuleResultFileSystem.IsSelected(file.Directory);
            using var _ = ImRaii.PushColor(ImGuiCol.Text, isSelected ? ImGuiColors.DalamudWhite : ImGuiColors.DalamudGrey3);
            using var __ = ImRaii.TreeNode($"{file.Name}###resultVirtualFile{file.GetHashCode()}", ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Bullet);
        }
    }
}
