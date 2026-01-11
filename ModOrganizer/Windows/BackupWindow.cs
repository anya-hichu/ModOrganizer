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
using ModOrganizer.Windows.Results;
using ModOrganizer.Windows.Results.Backups;
using ModOrganizer.Windows.Results.Showables;
using Scriban;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ModOrganizer.Windows;

public class BackupWindow : Window
{
    private IBackupManager BackupManager { get; init; }
    private IConfig Config { get; init; }

    private BackupResultState BackupResultState { get; init; }
    private TemplateContext ViewTemplateContext { get; init; } = new();


    public BackupWindow(IBackupManager backupManager, BackupResultState backupResultState, IConfig config, IModInterop modInterop, IPluginLog pluginLog) : base("ModOrganizer - Backup##backupWindow")
    {
        BackupManager = backupManager;
        BackupResultState = backupResultState;
        Config = config;

        SizeConstraints = new()
        {
            MinimumSize = new(375, 330),
            MaximumSize = new(float.MaxValue, float.MaxValue)
        }; 
    }

    public override void Draw()
    {
        if (ImGui.Button("Create##createBackup") && BackupManager.TryCreate(out var newBackup)) BackupResultState.Select(newBackup);
        ImGui.SameLine(ImGui.GetWindowWidth() - 95);
        if (ImGui.Button("Open Folder##openBackupFolder")) Process.Start("explorer", BackupManager.GetFolderPath());

        var hasResults = BackupResultState.GetResults().Any();
        var hasCompletedTask = BackupResultState.HasCompletedTask();

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

                var orderedBackups = Config.Backups.OrderDescending().ToList();

                foreach (var backup in orderedBackups)
                {
                    var hash = backup.GetHashCode();

                    var isSelected = backup == BackupResultState.Selected;

                    using (ImRaii.PushColor(ImGuiCol.Text, CustomColors.LightBlue, isSelected))
                    {
                        if (ImGui.TableNextColumn())
                        {
                            ImGui.Text(backup.Auto ? "Auto" : "Manual");
                            if (hasCompletedTask && ImGui.IsItemClicked()) BackupResultState.Select(backup);
                        }

                        if (ImGui.TableNextColumn())
                        {
                            ImGui.Text(backup.CreatedAt.Humanize());
                            if (ImGui.IsItemHovered()) ImGui.SetTooltip(backup.CreatedAt.ToLocalTime().ToString());
                            if (hasCompletedTask && ImGui.IsItemClicked()) BackupResultState.Select(backup);
                        }

                        if (ImGui.TableNextColumn())
                        {
                            var path = BackupManager.GetPath(backup);
                            using var _ = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudRed, !File.Exists(path));

                            ImGui.Text(BackupManager.GetFileName(backup));
                            if (ImGui.IsItemHovered()) ImGui.SetTooltip(path);
                            if (hasCompletedTask && ImGui.IsItemClicked()) BackupResultState.Select(backup);
                        }
                    }

                    if (ImGui.TableNextColumn())
                    {
                        if (ImGui.Button($"Select###selectBackup{hash}")) BackupResultState.Select(backup);
                        ImGui.SameLine();

                        using (ImRaii.Disabled(!hasCompletedTask || !isConfirmPressed))
                        {
                            using var _ = ImRaii.PushColor(ImGuiCol.Button, ImGuiColors.DalamudRed);
                            if (ImGui.Button($"Delete###deleteBackup{hash}") && BackupManager.TryDelete(backup) && isSelected) BackupResultState.Unselect();
                        }
                        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled)) ImGui.SetTooltip(Texts.ConfirmHint);
                    }
                }
            }
        }

        if (!hasResults) return;

        // TODO: SHOW empty folders to compare

        using (ImRaii.Disabled(!hasCompletedTask || !isConfirmPressed))
        {
            using ImRaii.Color? _ = ImRaii.PushColor(ImGuiCol.Button, ImGuiColors.ParsedGreen), __ = ImRaii.PushColor(ImGuiCol.Text, CustomColors.Black);
            if (ImGui.Button("Restore##applyBackupState")) BackupResultState.Apply();
        }
        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled)) ImGui.SetTooltip(Texts.ConfirmHint);

        ImGui.SameLine();
        var reloadPenumbra = BackupResultState.ReloadPenumbra;
        if (ImGui.Checkbox("Reload Penumbra##reloadPenumbra", ref reloadPenumbra)) BackupResultState.ReloadPenumbra = reloadPenumbra;
        if (ImGui.IsItemHovered()) ImGui.SetTooltip($"Automatically dispatch [{ModInterop.RELOAD_PENUMBRA_COMMAND}] command after restore (might take a few seconds for the file watchers to see changes)");


        ImGui.SameLine(ImGui.GetWindowWidth() - 190);
        var showSamePaths = BackupResultState.ShowSamePaths;
        if (ImGui.Checkbox("Show Same Paths##showBackupStateSamePath", ref showSamePaths)) BackupResultState.ShowSamePaths = showSamePaths;

        ImGui.SameLine();
        if (ImGui.Button("Clear##clearBackupState")) BackupResultState.Clear();

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
                var directoryFilter = BackupResultState.DirectoryFilter;
                ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - clearButtonWidth);
                if (ImGui.InputTextWithHint("##backupStateDirectoryFilter", Texts.FilterHint, ref directoryFilter, ushort.MaxValue)) BackupResultState.DirectoryFilter = directoryFilter;
                ImGui.SameLine();
                if (ImGui.Button("X##clearBackupStateDirectoryFilter")) BackupResultState.DirectoryFilter = string.Empty;
            }

            if (ImGui.TableNextColumn())
            {
                var pathFilter = BackupResultState.PathFilter;
                ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - clearButtonWidth);
                if (ImGui.InputTextWithHint("##backupStatePathFilter", Texts.FilterHint, ref pathFilter)) BackupResultState.PathFilter = pathFilter;
                ImGui.SameLine();
                if (ImGui.Button("X##clearBackupStatePathFilter")) BackupResultState.PathFilter = string.Empty;
            }

            if (ImGui.TableNextColumn())
            {
                var oldPathFilter = BackupResultState.PathFilter;
                ImGui.SetNextItemWidth(ImGui.GetColumnWidth() - clearButtonWidth);
                if (ImGui.InputTextWithHint("##backupStateOldPathFilter", Texts.FilterHint, ref oldPathFilter)) BackupResultState.OldPathFilter = oldPathFilter;
                ImGui.SameLine();
                if (ImGui.Button("X##clearBackupStateOldPathFilter")) BackupResultState.OldPathFilter = string.Empty;
            }

            var showedBackupResults = BackupResultState.GetShowedBackupResults().Order().ToList();

            using var imRaiiListClipper = new ImRaiiListClipper(showedBackupResults.Count, ImGui.GetTextLineHeightWithSpacing());
            var imGuiListClipper = imRaiiListClipper.Value;

            while (imGuiListClipper.Step())
            {
                for (var i = imGuiListClipper.DisplayStart; i < imGuiListClipper.DisplayEnd; i++)
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
