using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Common.Lua;
using ModOrganizer.Backups;
using ModOrganizer.Mods;
using ModOrganizer.Shared;
using ModOrganizer.Windows.States;
using ModOrganizer.Windows.States.Results;
using ModOrganizer.Windows.States.Results.Backups;
using ModOrganizer.Windows.States.Results.Rules;
using ModOrganizer.Windows.States.Results.Showables;
using Scriban;
using System;
using System.Diagnostics;
using System.Linq;

namespace ModOrganizer.Windows;

public class BackupWindow : Window, IDisposable
{
    // Add backup creation/restore + view to preview the diffs

    private BackupManager BackupManager { get; init; }
    private Config Config { get; init; }

    private BackupState BackupState { get; init; }
    private TemplateContext ViewTemplateContext { get; init; } = new();


    public BackupWindow(BackupManager backupManager, Config config, ModInterop ModInterop, IPluginLog pluginLog) : base("ModOrganizer - Backup##backupWindow")
    {
        SizeConstraints = new()
        {
            MinimumSize = new(375, 330),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        };

        BackupManager = backupManager;
        Config = config;

        BackupState = new(BackupManager, ModInterop, pluginLog);
    }

    public void Dispose() => BackupState.Dispose();


    public override void Draw()
    {
        if (ImGui.Button("Create Backup##createBackup") && BackupManager.TryCreate(out var newBackup)) BackupState.Select(newBackup);

        if (ImGui.Button("Open Backups Folder##openBackupFolder")) Process.Start("explorer", BackupManager.GetBackupsFolderPath());


        var hasResults = BackupState.GetResults().Any();

        var availableRegion = ImGui.GetContentRegionAvail();
        using (var backupsTable = ImRaii.Table("backupsTable", 3, ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY | ImGuiTableFlags.Resizable, hasResults ? new(availableRegion.X, availableRegion.Y / 2) : availableRegion))
        {
            if (backupsTable)
            {
                ImGui.TableSetupColumn($"Created At##backupCreatedAt", ImGuiTableColumnFlags.None, 1);
                ImGui.TableSetupColumn($"File Name##backupFileName", ImGuiTableColumnFlags.None, 1);
                ImGui.TableSetupColumn($"Actions##backupActions", ImGuiTableColumnFlags.None, 3);
                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableHeadersRow();

                // TODO: Add clipper
                foreach (var backup in Config.Backups.OrderDescending())
                {
                    if (ImGui.TableNextColumn()) ImGui.Text(backup.CreatedAt.ToString());
                    if (ImGui.TableNextColumn()) ImGui.Text(backup.FileName);
                    if (ImGui.TableNextColumn())
                    {
                        if (ImGui.Button("Select##selectBackup")) BackupState.Select(backup);

                        using (ImRaii.PushColor(ImGuiCol.Button, ImGuiColors.DalamudRed))
                        {
                            using var _ = ImRaii.Disabled(!ImGui.GetIO().KeyCtrl);
                            if (ImGui.Button("Delete##deleteBackup") && BackupManager.TryDelete(backup)) BackupState.Deselect(backup);  
                            if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled)) ImGui.SetTooltip(Texts.CtrlConfirmHint);
                        }
                    }
                }
            }
        }

        if (hasResults)
        {
            using (ImRaii.PushColor(ImGuiCol.Button, ImGuiColors.DalamudRed))
            {
                using var _ = ImRaii.Disabled(!ImGui.GetIO().KeyCtrl);
                if (ImGui.Button("Restore##restoreBackup")) BackupState.Restore();
                if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled)) ImGui.SetTooltip(Texts.CtrlConfirmHint);
            }

            var showSamePaths = BackupState.ShowSamePaths;
            if (ImGui.Checkbox("Show Same Paths##showSamePathBackupResults", ref showSamePaths)) BackupState.ShowSamePaths = showSamePaths;

            using var backupsTable = ImRaii.Table("backupResultsTable", 3, ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY | ImGuiTableFlags.Resizable, new(availableRegion.X, availableRegion.Y / 2));
            if (backupsTable)
            {
                ImGui.TableSetupColumn($"Mod Directory##backupCreatedAt", ImGuiTableColumnFlags.None, 2);
                ImGui.TableSetupColumn($"Path##backupFileName", ImGuiTableColumnFlags.None, 3);
                ImGui.TableSetupColumn($"New Path##backupActions", ImGuiTableColumnFlags.None, 3);
                ImGui.TableSetupScrollFreeze(0, 2);
                ImGui.TableHeadersRow();

                var buttonWidth = ImGui.CalcTextSize("NNNN").X;
                if (ImGui.TableNextColumn())
                {
                    var directoryFilter = BackupState.DirectoryFilter;
                    ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - buttonWidth);
                    if (ImGui.InputTextWithHint("##backupDirectoryFilter", Texts.FilterHint, ref directoryFilter, ushort.MaxValue)) BackupState.DirectoryFilter = directoryFilter;
                    ImGui.SameLine();
                    if (ImGui.Button("X###clearBackupDirectoryFilter")) BackupState.DirectoryFilter = string.Empty;
                }

                if (ImGui.TableNextColumn())
                {
                    var pathFilter = BackupState.PathFilter;
                    ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - buttonWidth);
                    if (ImGui.InputTextWithHint("##backupPathFilter", Texts.FilterHint, ref pathFilter)) BackupState.PathFilter = pathFilter;
                    ImGui.SameLine();
                    if (ImGui.Button("X##clearBackupPathFilter")) BackupState.PathFilter = string.Empty;
                }

                if (ImGui.TableNextColumn())
                {
                    var newPathFilter = BackupState.NewPathFilter;
                    ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - buttonWidth);
                    if (ImGui.InputTextWithHint("##backupNewPathFilter", Texts.FilterHint, ref newPathFilter)) BackupState.NewPathFilter = newPathFilter;
                    ImGui.SameLine();
                    if (ImGui.Button("X##clearBackupNewPathFilter")) BackupState.NewPathFilter = string.Empty;
                }

                var showedBackupResults = BackupState.GetShowedResults<BackupResult, IShowableBackupResultState>().Order();

                var clipper = ImGui.ImGuiListClipper();
                clipper.Begin(showedBackupResults.Count(), ImGui.GetTextLineHeightWithSpacing());
                while (clipper.Step())
                {
                    for (var i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
                    {
                        var backupResult = showedBackupResults.ElementAt(i);

                        if (ImGui.TableNextColumn())
                        {
                            ImGui.Text(backupResult.Directory);
                            if (ImGui.IsItemHovered()) ImGui.SetTooltip(ViewTemplateContext.ObjectToString(backupResult.Directory, true));
                        }

                        using var _ = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudGrey3, backupResult is BackupSamePathResult);

                        if (ImGui.TableNextColumn())
                        {
                            DrawResultValue(backupResult.Path);
                        }

                        if (ImGui.TableNextColumn())
                        {
                            switch (backupResult)
                            {
                                case BackupPathResult backupPathResult:
                                    DrawResultValue(backupPathResult.NewPath);
                                    break;
                                case BackupSamePathResult ruleSamePathResult:
                                    DrawResultValue(ruleSamePathResult.Path);
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }

    private void DrawResultValue(string? value)
    {
        if (value == null)
        {
            using var _ = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudRed);
            ImGui.Text("deleted");
        }
        else
        {
            ImGui.Text(value);
        }

        if (ImGui.IsItemHovered()) ImGui.SetTooltip(ViewTemplateContext.ObjectToString(value, true));
    }

}
