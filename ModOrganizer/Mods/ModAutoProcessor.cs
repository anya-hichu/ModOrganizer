using Dalamud.Plugin.Services;
using ModOrganizer.Configs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ModOrganizer.Mods;

public class ModAutoProcessor : IModAutoProcessor
{
    private IConfig Config { get; init; }
    private INotificationManager NotificationManager { get; init; }
    private IModInterop ModInterop { get; init; }
    private IModProcessor ModProcessor { get; init; }
    private IPluginLog PluginLog { get; init; }
    private HashSet<Task> RunningTasks { get; init; } = [];

    public ModAutoProcessor(IConfig config, INotificationManager notificationManager, IModInterop modInterop, IModProcessor modProcessor, IPluginLog pluginLog)
    {
        Config = config;
        NotificationManager = notificationManager;
        ModInterop = modInterop;
        ModProcessor = modProcessor;
        PluginLog = pluginLog;

        ModInterop.OnModAdded += ProcessIfEnabled;
    } 

    public void Dispose() => ModInterop.OnModAdded -= ProcessIfEnabled;

    private void ProcessIfEnabled(string modDirectory)
    {
        if (!Config.AutoProcessEnabled) return;

        var task = Task.Run(() =>
        {
            PluginLog.Debug($"Waiting [{Config.AutoProcessDelayMs}] ms before processing mod [{modDirectory}] inside task [{Task.CurrentId}]");
            Thread.Sleep(Convert.ToInt32(Config.AutoProcessDelayMs));

            if (!ModProcessor.TryProcess(modDirectory, out var newModPath)) return;

            NotificationManager.AddNotification(new() { Title = nameof(ModOrganizer), MinimizedText = $"Updated mod [{modDirectory}] path to [{newModPath}]" });
        }).ContinueWith(RunningTasks.Remove);

        RunningTasks.Add(task);
    }

    public IEnumerable<Task> GetRunningTasks() => RunningTasks;
}
