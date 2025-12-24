using Dalamud.Plugin.Services;
using ModOrganizer.Configs;
using System;
using System.Threading.Tasks;

namespace ModOrganizer.Mods;

public class ModAutoProcessor : IModAutoProcessor
{
    private IConfig Config { get; init; }
    private INotificationManager NotificationManager { get; init; }
    private IModInterop ModInterop { get; init; }
    private IModProcessor ModProcessor { get; init; }
    private IPluginLog PluginLog { get; init; }
    private Task Task { get; set; } = Task.CompletedTask;
    
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

    public Task GetCurrentTask() => Task;

    private void ProcessIfEnabled(string modDirectory)
    {
        if (!Config.AutoProcessEnabled) return;
        PluginLog.Debug($"Waiting [{Config.AutoProcessDelayMs}] ms before processing mod [{modDirectory}]");

        Task = Task.Delay(Convert.ToInt32(Config.AutoProcessDelayMs)).ContinueWith(_ =>
        {
            if (!ModProcessor.TryProcess(modDirectory, out var newModPath)) return;

            NotificationManager.AddNotification(new() { Title = nameof(ModOrganizer), MinimizedText = $"Updated mod [{modDirectory}] path to [{newModPath}]" });
        }); 
    }
}
