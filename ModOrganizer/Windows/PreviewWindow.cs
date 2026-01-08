using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ModOrganizer.Shared;
using ModOrganizer.Virtuals;
using ModOrganizer.Windows.Results;
using ModOrganizer.Windows.Results.Rules;
using System;
using System.Linq;

namespace ModOrganizer.Windows;

public class PreviewWindow : Window
{
    private RuleResultFileSystem RuleResultFileSystem { get; init; }

    private string Filter { get; set; } = string.Empty;
    public bool ShowUnselected { get; set; } = false;

    public PreviewWindow(RuleResultFileSystem ruleResultFileSystem) : base("ModOrganizer - Preview##previewWindow")
    {
        SizeConstraints = new()
        {
            MinimumSize = new(375, 330),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };

        RuleResultFileSystem = ruleResultFileSystem;
    }

    public override void Draw()
    {
        var filter = Filter;
        if (ImGui.InputTextWithHint("##resultFilter", Texts.FilterHint, ref filter)) Filter = filter;
        ImGui.SameLine();
        if (ImGui.Button("X##clearResultFilter")) Filter = string.Empty;

        ImGui.SameLine(ImGui.GetWindowWidth() - 130);
        var showUnselected = ShowUnselected;
        if (ImGui.Checkbox("Show Unselected##showUnselectedResults", ref showUnselected)) ShowUnselected = showUnselected;

        if (RuleResultFileSystem.GetRootFolder().TrySearch(GetMatcher(), out var filteredFolder)) DrawVirtualFolderTree(filteredFolder);
    }

    private void DrawVirtualFolderTree(VirtualFolder folder)
    {
        foreach (var subfolder in folder.Folders.Order())
        {
            using var _ = ImRaii.PushColor(ImGuiCol.Text, CustomColors.LightBlue);
            using var treeNode = ImRaii.TreeNode($"{subfolder.Name}###resultVirtualFolder{subfolder.GetHashCode()}");
            if (treeNode) DrawVirtualFolderTree(subfolder);
        }

        foreach (var file in folder.Files.Order())
        {
            if (!RuleResultFileSystem.TryGetFileData(file, out var rulePathResult)) continue;
            using var _ = ImRaii.PushColor(ImGuiCol.Text, rulePathResult.Selected ? ImGuiColors.DalamudWhite : ImGuiColors.DalamudGrey3);
            using var node = ImRaii.TreeNode($"{file.Name}###resultVirtualFile{file.GetHashCode()}", ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Bullet);
            if (ImGui.IsItemClicked()) rulePathResult.Selected = !rulePathResult.Selected;;
            if (ImGui.IsItemHovered()) ImGui.SetTooltip($"Current: {rulePathResult.Path}");
        }
    }

    private VirtualMultiMatcher GetMatcher() => new([
        new VirtualAttributesMatcher(Filter), 
        new RuleResultMatcher(RuleResultFileSystem, ShowUnselected)
    ]);
}
