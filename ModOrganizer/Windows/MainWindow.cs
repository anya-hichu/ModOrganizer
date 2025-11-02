using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using ModOrganizer.Mods;
using ModOrganizer.Utils;
using Scriban;
using Scriban.Helpers;
using Scriban.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace ModOrganizer.Windows;

public class MainWindow : Window, IDisposable
{
    private static readonly Vector4 LIGHT_BLUE = new(0.753f, 0.941f, 1, 1);

    private ModInterop ModInterop { get; init; }
    private ModVirtualFileSystem ModVirtualFileSystem { get; init; }

    private TemplateContext TemplateContext { get; init; } = new(){ MemberRenamer = ScribanUtils.RenameMember };
    private SourceSpan SourceSpan { get; init; } = new();

    private string Filter { get; set; } = string.Empty;
    private HashSet<string> SelectedModDirectories { get; set; } = [];

    private string Expression { get; set; } = string.Empty;
    private object? EvaluationResult { get; set; }

    public MainWindow(ModInterop modInterop, ModVirtualFileSystem modVirtualFileSystem) : base("ModOrganizer - Main##mainWindow")
    {
        SizeConstraints = new()
        {
            MinimumSize = new(375, 330),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };

        ModInterop = modInterop;
        ModVirtualFileSystem = modVirtualFileSystem;
    }

    public void Dispose() => ModInterop.EnableFileSystemWatchers(false);

    public override void OnOpen() => ModInterop.EnableFileSystemWatchers(true);

    public override void OnClose() => ModInterop.EnableFileSystemWatchers(false);

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
            
            if (!ImGui.IsItemClicked()) continue;

            // TODO: fix
            if (ImGui.IsKeyDown(ImGuiKey.LeftShift) && SelectedModDirectories.Count == 1)
            {
                var j = Array.IndexOf(orderedFiles, SelectedModDirectories.First());
                if (j != -1) SelectedModDirectories.UnionWith(orderedFiles[i..j].Select(f => f.Directory));
                continue;
            } 

            if (!ImGui.IsKeyDown(ImGuiKey.LeftCtrl)) SelectedModDirectories.Clear();

            // Toggle
            if (!SelectedModDirectories.Remove(file.Directory)) SelectedModDirectories.Add(file.Directory);
        }
    }

    public override void Draw()
    {
        using (ImRaii.PushColor(ImGuiCol.ChildBg, new Vector4(0.2f, 0.2f, 0.2f, 0.5f)))
        {
            using (ImRaii.Child("modVirtualFileSystem", new(ImGui.GetWindowWidth() * 0.2f, ImGui.GetWindowHeight() - (2 * ImGui.GetTextLineHeightWithSpacing()))))
            {
                var filter = Filter;
                if (ImGui.InputText("###filter", ref filter))
                {
                    Filter = filter;
                }
                ImGui.SameLine();
                if (ImGui.Button("X###clearFilter"))
                {
                    Filter = string.Empty;
                }

                if (ModVirtualFileSystem.GetRootFolder().TrySearch(filter, out var filteredFolder)) 
                {
                    DrawVirtualFolderTree(filteredFolder);
                }
            }
        }


        ImGui.SameLine();

        if (SelectedModDirectories.Count == 0)
        {
            ImGui.Text("No mod selected");
            ImGuiComponents.HelpMarker("Click on the left panel to select one and hold <CTRL> for multi-selection");
        }
        else if (SelectedModDirectories.Count == 1)
        {
            var selectedModDirectory = SelectedModDirectories.First();
            var modInfo = ModInterop.GetModInfo(selectedModDirectory);
            using (ImRaii.PushColor(ImGuiCol.ChildBg, new Vector4(0.2f, 0.2f, 0.2f, 0.5f)))
            {
                using (ImRaii.Child($"selectedModDirectory", new((ImGui.GetWindowWidth() * 0.8f) - (2 * ImGui.GetTextLineHeight()), ImGui.GetWindowHeight() * 0.7f)))
                {
                    DrawMembers(modInfo, 3);
                }
            }

            ImGui.SetCursorPos(new((ImGui.GetWindowWidth() * 0.2f) + ImGui.GetTextLineHeight(), (ImGui.GetWindowHeight() * 0.7f) + (2 * ImGui.GetTextLineHeightWithSpacing())));
            using (ImRaii.Child($"actions", new((ImGui.GetWindowWidth() * 0.8f) - (2 * ImGui.GetTextLineHeight()), (ImGui.GetWindowHeight() * 0.3f) - (3 * ImGui.GetTextLineHeight()))))
            {
                var expression = Expression;
                if (ImGui.InputTextMultiline("Expression##expression", ref expression))
                {
                    Expression = expression;
                }

                if (ImGui.Button("Evaluate##evaluate"))
                {
                    EvaluationResult = Template.Evaluate(Expression, modInfo, ScribanUtils.RenameMember);
                }

                if (EvaluationResult != null)
                {
                    ImGui.Text($"{TemplateContext.ObjectToString(EvaluationResult)} ({EvaluationResult.GetType().ScriptPrettyName()})");
                    if (ImGui.Button("Clear##clear"))
                    {
                        EvaluationResult = null;
                    }
                }
            }


        }
        else
        {
            // Table
            ImGui.Text("TODO");
        }
        // button to run evaluate all rules (Dry run)
        // Button evaluate for real



    }

    private void DrawMembers(object target, int maxDepth)
    {
        // Change to infinite depth and tree node items (lazy)
        // Add support for propery list display

        var accessor = TemplateContext.GetMemberAccessor(target);
        using (ImRaii.PushIndent())
        {
            foreach (var member in accessor.GetMembers(TemplateContext, SourceSpan, target))
            {
                if (accessor.TryGetValue(TemplateContext, SourceSpan, target, member, out var memberValue))
                {
                    ImGui.Text($"{member}: {TemplateContext.ObjectToString(memberValue)} ({memberValue.GetType().ScriptPrettyName()})");
                    if (maxDepth > 0)
                    {
                        DrawMembers(memberValue, maxDepth - 1);
                    }
                }
            }
        }
    }
}
