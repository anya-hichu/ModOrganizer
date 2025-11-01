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
    private ModInterop ModInterop { get; init; }
    private ModVirtualFileSystem ModVirtualFileSystem { get; init; }

    private TemplateContext TemplateContext { get; init; } = new(){ MemberRenamer = ScribanUtils.RenameMember };
    private SourceSpan SourceSpan { get; init; } = new();

    private HashSet<string> SelectedModDirectoryPaths { get; set; } = [];

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

    private void DrawVirtualFolder(ModVirtualFolder folder)
    {
        foreach (var subfolder in folder.Folders.OrderBy(f => f.Name))
        {
            var hash = subfolder.GetHashCode();
            using var treeNode = ImRaii.TreeNode($"{subfolder.Name}###folder{hash}");
            if (treeNode) DrawVirtualFolder(subfolder);
        }

        foreach (var file in folder.Files.OrderBy(f => f.Name))
        {
            var hash = file.GetHashCode();
            using (ImRaii.TreeNode($"{file.Name}###file{hash}", ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Bullet | (SelectedModDirectoryPaths.Contains(file.Directory) ? ImGuiTreeNodeFlags.Selected : ImGuiTreeNodeFlags.None)))
            {
                using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudWhite))
                {
                    if (ImGui.IsItemClicked())
                    {
                        if (!ImGui.IsKeyDown(ImGuiKey.LeftCtrl)) SelectedModDirectoryPaths.Clear();

                        // Toggle
                        if (!SelectedModDirectoryPaths.Remove(file.Directory)) SelectedModDirectoryPaths.Add(file.Directory);
                    }
                }
            }

        }
    }

    public override void Draw()
    {
        using (ImRaii.PushColor(ImGuiCol.ChildBg, new Vector4(0.2f, 0.2f, 0.2f, 0.5f)))
        {
            using (ImRaii.Child("modVirtualFileSystem", new(ImGui.GetWindowWidth() * 0.2f, ImGui.GetWindowHeight() - (2 * ImGui.GetTextLineHeightWithSpacing()))))
            {
                DrawVirtualFolder(ModVirtualFileSystem.GetRootFolder());
            }
        }


        ImGui.SameLine();

        if (SelectedModDirectoryPaths.Count == 0)
        {
            ImGui.Text("No mod selected");
            ImGuiComponents.HelpMarker("Click on the left panel to select one and hold <CTRL> for multi-selection");
        }
        else if (SelectedModDirectoryPaths.Count == 1)
        {
            var selectedModDirectory = SelectedModDirectoryPaths.First();
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
        // Change to infinite depth and tree node items

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
