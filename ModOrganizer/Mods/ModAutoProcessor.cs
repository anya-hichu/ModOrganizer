using Dalamud.Plugin.Services;
using ModOrganizer.Backups;
using System;
using System.Threading.Tasks;

namespace ModOrganizer.Mods;

public class ModAutoProcessor : IDisposable
{
    private IChatGui ChatGui { get; init; }
    private Config Config { get; init; }
    private ModInterop ModInterop { get; init; }
    private ModProcessor ModProcessor { get; init; }
    private IPluginLog PluginInfo { get; init; }
    
    public ModAutoProcessor(BackupManager backupManager, IChatGui chatGui, Config config, ModInterop modInterop, ModProcessor modProcessor, IPluginLog pluginInfo)
    {
        ChatGui = chatGui;
        Config = config;
        ModInterop = modInterop;
        ModProcessor = modProcessor;
        PluginInfo = pluginInfo;

        ModInterop.OnModAdded += ProcessIfEnabled;
    }

    public void Dispose() => ModInterop.OnModAdded -= ProcessIfEnabled;

    private void ProcessIfEnabled(string modDirectory)
    {
        if (!Config.AutoProcessEnabled) return;
        PluginInfo.Debug($"Waiting [{Config.AutoProcessWaitMs}] ms before processing mod [{modDirectory}]");

        Task.Delay((int)Config.AutoProcessWaitMs).ContinueWith(_ =>
        {
            if (ModProcessor.TryProcess(modDirectory, out var newModPath))
            {
                ChatGui.Print($"Set mod [{modDirectory}] path to [{newModPath}]", Plugin.NAMESPACE);
            }
        }); 
    }
}
