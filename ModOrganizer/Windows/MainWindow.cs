using Dalamud.Bindings.ImGui;
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
    private ModInterop Interop { get; init; }

    private TemplateContext TemplateContext { get; init; } = new()
    {
        MemberRenamer = ScribanUtils.RenameMember
    };
    private SourceSpan SourceSpan { get; init; } = new();

    private HashSet<string> SelectedModDirectories { get; set; } = [];


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
    }

    public void Dispose() { }

    public override void Draw()
    {
        using (ImRaii.PushColor(ImGuiCol.ChildBg, new Vector4(0.2f, 0.2f, 0.2f, 0.5f)))
        {
            using (ImRaii.Child("modDirectories", new(ImGui.GetWindowWidth() * 0.2f, ImGui.GetWindowHeight() - ImGui.GetCursorPosY() - 10)))
            {
                // Add filter

                foreach (var modDirectory in Interop.GetCachedModDirectories().Order())
                {
                    using (ImRaii.PushColor(ImGuiCol.Button, SelectedModDirectories.Contains(modDirectory) ? new Vector4(0.5f, 0.5f, 0.5f, 0.5f) : new Vector4(0.3f, 0.3f, 0.3f, 0.3f)))
                    {
                        if (ImGui.Button($"{modDirectory}##modDirectory{modDirectory.GetHashCode()}", new(ImGui.GetContentRegionAvail().X, ImGui.GetTextLineHeightWithSpacing())))
                        {
                            if (ImGui.IsKeyPressed(ImGuiKey.LeftCtrl))
                            {
                                if (!SelectedModDirectories.Add(modDirectory))
                                {
                                    SelectedModDirectories.Remove(modDirectory);
                                }
                            }
                            else
                            {
                                SelectedModDirectories.Clear();
                                SelectedModDirectories.Add(modDirectory);
                            }
                        }
                        if (ImGui.IsItemHovered())
                        {
                            ImGui.SetTooltip(modDirectory);
                        }
                    }
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
            var modInfo = Interop.GetCachedModInfo(selectedModDirectory);
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
