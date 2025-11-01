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
using System.IO;
using System.Linq;
using System.Numerics;

namespace ModOrganizer.Windows;

public class MainWindow : Window, IDisposable
{
    private ModInterop Interop { get; init; }

    private TemplateContext TemplateContext { get; init; } = new(){ MemberRenamer = ScribanUtils.RenameMember };
    private SourceSpan SourceSpan { get; init; } = new();

    private HashSet<string> SelectedModDirectories { get; set; } = [];
    private HashSet<TreeNode<string>>? MaybeModPathNodesCache { get; set; }

    private string Expression { get; set; } = string.Empty;
    private object? EvaluationResult { get; set; }

    public MainWindow(ModInterop interop) : base("ModOrganizer - Main##mainWindow")
    {
        SizeConstraints = new()
        {
            MinimumSize = new(375, 330),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };

        Interop = interop;
        Interop.OnModPathsChanged += OnModPathsChanged;
    }

    public void Dispose() 
    {
        Interop.OnModPathsChanged -= OnModPathsChanged;
        Interop.EnableRaisingFsEvents(false);
    }

    public override void OnOpen()
    {
        Interop.EnableRaisingFsEvents(true);
    }

    public override void OnClose()
    {
        Interop.EnableRaisingFsEvents(false);
    }

    private void OnModPathsChanged()
    {
        MaybeModPathNodesCache = null;
    }

    private HashSet<TreeNode<string>> GetModPathNodes()
    {
        if (MaybeModPathNodesCache != null) return MaybeModPathNodesCache;

        var modPathNodes = new HashSet<TreeNode<string>>(TreeNodeComparer<string>.INSTANCE);
        foreach (var (_, path) in Interop.GetModPaths())
        {
            var current = modPathNodes;
            var parts = path.Split('/');
            for (var take = 1; take <= parts.Length; take++)
            {
                var newNode = new TreeNode<string>(string.Join('/', parts.Take(take)));
                if (current.TryGetValue(newNode, out var existingNode))
                {
                    current = existingNode.ChildNodes;
                }
                else
                {
                    current.Add(newNode);
                    current = newNode.ChildNodes;
                }
            }
        }

        MaybeModPathNodesCache = modPathNodes;
        return modPathNodes;
    }

    private void DrawPathNodes(HashSet<TreeNode<string>> nodes)
    {
        // Pad with zeros to improve sorting with numbers
        foreach (var treeNode in nodes.OrderByDescending(n => n.ChildNodes.Count != 0).ThenBy(n => n.Node))
        {
            var hash = treeNode.Node.GetHashCode();

            var name = Path.GetFileName(treeNode.Node);
            if (treeNode.ChildNodes.Count > 0)
            {
                // Folder
                using var treeNodeItem = ImRaii.TreeNode($"{name}###pathNode{hash}");
                if (treeNodeItem) DrawPathNodes(treeNode.ChildNodes);
                continue;
            }

            // Leaf
            var modDirectory = treeNode.Node.Split('/').Last();
            using (ImRaii.TreeNode($"{modDirectory}###pathNode{hash}", ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Bullet | (SelectedModDirectories.Contains(modDirectory) ? ImGuiTreeNodeFlags.Selected : ImGuiTreeNodeFlags.None)))
            {
                using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudWhite))
                {
                    if (ImGui.IsItemClicked())
                    {
                        if (!ImGui.IsKeyDown(ImGuiKey.LeftCtrl)) SelectedModDirectories.Clear();

                        // Toggle
                        if (!SelectedModDirectories.Remove(modDirectory)) SelectedModDirectories.Add(modDirectory);
                    }
                }
            }
        }
    }

    public override void Draw()
    {
        using (ImRaii.PushColor(ImGuiCol.ChildBg, new Vector4(0.2f, 0.2f, 0.2f, 0.5f)))
        {
            using (ImRaii.Child("modPathNodes", new(ImGui.GetWindowWidth() * 0.2f, ImGui.GetWindowHeight() - (2 * ImGui.GetTextLineHeightWithSpacing()))))
            {
                DrawPathNodes(GetModPathNodes());
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
            var modInfo = Interop.GetModInfo(selectedModDirectory);
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
