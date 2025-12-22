using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using Humanizer;
using ModOrganizer.Backups;
using ModOrganizer.Configs;
using ModOrganizer.Mods;
using ModOrganizer.Shared;
using ModOrganizer.Windows.States;
using ModOrganizer.Windows.States.Results.Backups;
using ModOrganizer.Windows.States.Results.Showables;
using Scriban;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ModOrganizer.Windows;

public class BackupWindow : Window, IDisposable
{
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
        if (ImGui.Button("Create##createBackup") && BackupManager.TryCreate(out var newBackup)) BackupState.Select(newBackup);
        ImGui.SameLine(ImGui.GetWindowWidth() - 95);
        if (ImGui.Button("Open Folder##openBackupFolder")) Process.Start("explorer", BackupManager.GetFolderPath());

        var hasResults = BackupState.GetResults().Any();
        var isConfirmPressed = ImGui.GetIO().KeyCtrl;

        var availableRegion = ImGui.GetContentRegionAvail();
        using (var backupsTable = ImRaii.Table("backupsTable", 4, ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY | ImGuiTableFlags.Resizable, hasResults ? new(availableRegion.X, availableRegion.Y / 2) : availableRegion))
        {
            if (backupsTable)
            {
                ImGui.TableSetupColumn($"Type##backupType", ImGuiTableColumnFlags.None, 2);
                ImGui.TableSetupColumn($"Created##backupCreated", ImGuiTableColumnFlags.None, 2);
                ImGui.TableSetupColumn($"File Name##backupFileName", ImGuiTableColumnFlags.None, 3);
                ImGui.TableSetupColumn($"Actions##backupActions", ImGuiTableColumnFlags.None, 3);
                ImGui.TableSetupScrollFreeze(0, 1);
                ImGui.TableHeadersRow();

                foreach (var backup in Config.Backups.OrderDescending().ToList())
                {
                    var hash = backup.GetHashCode();

                    var isSelected = backup == BackupState.Selected;

                    using (ImRaii.PushColor(ImGuiCol.Text, CustomColors.LightBlue, isSelected))
                    {
                        if (ImGui.TableNextColumn())
                        {
                            ImGui.Text(backup.Auto ? "Auto" : "Manual");
                            if (ImGui.IsItemClicked()) BackupState.Select(backup);
                        }

                        if (ImGui.TableNextColumn())
                        {
                            ImGui.Text(backup.CreatedAt.Humanize());
                            if (ImGui.IsItemHovered()) ImGui.SetTooltip(backup.CreatedAt.ToLocalTime().ToString());
                            if (ImGui.IsItemClicked()) BackupState.Select(backup);
                        }

                        if (ImGui.TableNextColumn())
                        {
                            var path = BackupManager.GetPath(backup);
                            using var _ = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudRed, !File.Exists(path));

                            ImGui.Text(BackupManager.GetFileName(backup.CreatedAt));
                            if (ImGui.IsItemHovered()) ImGui.SetTooltip(path);
                            if (ImGui.IsItemClicked()) BackupState.Select(backup);
                        }
                    }

                    if (ImGui.TableNextColumn())
                    {
                        if (ImGui.Button($"Select###selectBackup{hash}")) BackupState.Select(backup);
                        ImGui.SameLine();

                        using (ImRaii.Disabled(!isConfirmPressed))
                        {
                            using var _ = ImRaii.PushColor(ImGuiCol.Button, ImGuiColors.DalamudRed);
                            if (ImGui.Button($"Delete###deleteBackup{hash}") && BackupManager.TryDelete(backup) && isSelected) BackupState.Unselect();
                        }
                        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled)) ImGui.SetTooltip(Texts.ConfirmHint);
                    }
                }
            }
        }

        if (hasResults)
        {
            using (ImRaii.Disabled(!isConfirmPressed))
            {
                using ImRaii.Color? _ = ImRaii.PushColor(ImGuiCol.Button, ImGuiColors.ParsedGreen), __ = ImRaii.PushColor(ImGuiCol.Text, CustomColors.Black);
                if (ImGui.Button("Restore##applyBackupState")) BackupState.Apply();
            }
            if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled)) ImGui.SetTooltip(Texts.ConfirmHint);

            ImGui.SameLine();
            var reloadPenumbra = BackupState.ReloadPenumbra;
            if (ImGui.Checkbox("Reload Penumbra##reloadPenumbra", ref reloadPenumbra)) BackupState.ReloadPenumbra = reloadPenumbra;
            if (ImGui.IsItemHovered()) ImGui.SetTooltip($"Automatically dispatch [{ModInterop.RELOAD_PENUMBRA_COMMAND}] command after restore (might take a few seconds for the file watchers to see changes)");


            ImGui.SameLine(ImGui.GetWindowWidth() - 190);
            var showSamePaths = BackupState.ShowSamePaths;
            if (ImGui.Checkbox("Show Same Paths##showBackupStateSamePath", ref showSamePaths)) BackupState.ShowSamePaths = showSamePaths;

            ImGui.SameLine();
            if (ImGui.Button("Clear##clearBackupState")) BackupState.Clear();

            using var backupStateResultsTable = ImRaii.Table("backupStateResultsTable", 3, ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY | ImGuiTableFlags.Resizable);
            if (backupStateResultsTable)
            {
                ImGui.TableSetupColumn($"Mod Directory##backupStateResultDirectories", ImGuiTableColumnFlags.None, 2);
                ImGui.TableSetupColumn($"Path##backupStateResultPathPaths", ImGuiTableColumnFlags.None, 3);
                ImGui.TableSetupColumn($"Old Path##backupStateResultOldPaths", ImGuiTableColumnFlags.None, 3);
                ImGui.TableSetupScrollFreeze(0, 2);
                ImGui.TableHeadersRow();

                var clearButtonWidth = ImGui.CalcTextSize("NNNN").X;
                if (ImGui.TableNextColumn())
                {
                    var directoryFilter = BackupState.DirectoryFilter;
                    ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - clearButtonWidth);
                    if (ImGui.InputTextWithHint("##backupStateDirectoryFilter", Texts.FilterHint, ref directoryFilter, ushort.MaxValue)) BackupState.DirectoryFilter = directoryFilter;
                    ImGui.SameLine();
                    if (ImGui.Button("X##clearBackupStateDirectoryFilter")) BackupState.DirectoryFilter = string.Empty;
                }

                if (ImGui.TableNextColumn())
                {
                    var pathFilter = BackupState.PathFilter;
                    ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - clearButtonWidth);
                    if (ImGui.InputTextWithHint("##backupStatePathFilter", Texts.FilterHint, ref pathFilter)) BackupState.PathFilter = pathFilter;
                    ImGui.SameLine();
                    if (ImGui.Button("X##clearBackupStatePathFilter")) BackupState.PathFilter = string.Empty;
                }

                if (ImGui.TableNextColumn())
                {
                    var oldPathFilter = BackupState.PathFilter;
                    ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - clearButtonWidth);
                    if (ImGui.InputTextWithHint("##backupStateOldPathFilter", Texts.FilterHint, ref oldPathFilter)) BackupState.OldPathFilter = oldPathFilter;
                    ImGui.SameLine();
                    if (ImGui.Button("X##clearBackupStateOldPathFilter")) BackupState.OldPathFilter = string.Empty;
                }

                var showedBackupResults = BackupState.GetShowedResults<BackupResult, IShowableBackupResultState>().Order();

                using var clipperResource = new ImRaiiListClipper();

                var clipper = clipperResource.Value;
                clipper.Begin(showedBackupResults.Count(), ImGui.GetTextLineHeightWithSpacing());
                while (clipper.Step())
                {
                    for (var i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
                    {
                        var backupResult = showedBackupResults.ElementAt(i);

                        if (ImGui.TableNextColumn())
                        {
                            ImGui.Text(backupResult.Directory);
                            if (ImGui.IsItemHovered()) ImGui.SetTooltip(Inspect(backupResult.Directory));
                        }

                        using var _ = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudGrey3, backupResult is BackupSamePathResult);

                        if (ImGui.TableNextColumn()) DrawResultValue(backupResult.Path);

                        if (ImGui.TableNextColumn())
                        {
                            switch (backupResult)
                            {
                                case BackupPathResult backupPathResult:
                                    DrawResultValue(backupPathResult.OldPath);
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
            return;
        }

        ImGui.Text(value);
        if (ImGui.IsItemHovered()) ImGui.SetTooltip(Inspect(value));
    }

    private string Inspect(object? value) => ViewTemplateContext.ObjectToString(value, true);

}
