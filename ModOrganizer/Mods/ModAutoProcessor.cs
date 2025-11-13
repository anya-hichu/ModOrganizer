using Dalamud.Plugin.Services;
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
    
    public ModAutoProcessor(IChatGui chatGui, Config config, ModInterop modInterop, ModProcessor modProcessor, IPluginLog pluginInfo)
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
        PluginInfo.Debug($"Waiting [{Config.AutoProcessDelayMs}] ms before processing mod [{modDirectory}]");

        Task.Delay((int)Config.AutoProcessDelayMs).ContinueWith(_ =>
        {
            if (ModProcessor.TryProcess(modDirectory, out var newModDirectory))
            {
                ChatGui.Print($"Moved mod [{modDirectory}] to [{newModDirectory}]", Plugin.NAMESPACE);
            }
        }); 
    }
}
