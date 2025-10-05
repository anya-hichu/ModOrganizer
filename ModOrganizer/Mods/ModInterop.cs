using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ModOrganizer.Utils;
using Penumbra.Api.Enums;
using Penumbra.Api.Helpers;
using Penumbra.Api.IpcSubscribers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace ModOrganizer.Mods;

public class ModInterop : IDisposable
{
    private IPluginLog PluginLog { get; init; }


    public event Action<string>? OnModAdded;

    private HashSet<string>? ModDirectoriesCache { get; set; }
    private Dictionary<string, ModInfo> ModInfoCache { get; set; } = [];
    private Dictionary<string, string> ModPathsCache { get; set; } = [];

    private GetModDirectory GetModDirectorySubscriber { get; init; }
    private GetModPath GetModPathSubscriber { get; init; }
    private GetModList GetModListSubscriber { get; init; }
    private GetChangedItems GetChangedItemsSubscriber { get; init; }
    private SetModPath SetModPathSubscriber { get; init; }

    private EventSubscriber<string> ModAddedSubscriber { get; init; }
    private EventSubscriber<string> ModDeletedSubscriber { get; init; }
    private EventSubscriber<string, string> ModMovedSubscriber { get; init; }

    private string PluginConfigsDirectory { get; init; }
    private string LocalDataConfigPathTemplate { get; init; }

    public ModInterop(IDalamudPluginInterface pluginInterface, IPluginLog pluginLog)
    {
        PluginLog = pluginLog;

        GetModDirectorySubscriber = new(pluginInterface);
        GetModPathSubscriber = new(pluginInterface);
        GetModListSubscriber = new(pluginInterface);
        GetChangedItemsSubscriber = new(pluginInterface);
        SetModPathSubscriber = new(pluginInterface);

        ModAddedSubscriber = ModAdded.Subscriber(pluginInterface, modDirectory =>
        {
            ModDirectoriesCache = null;
            OnModAdded?.Invoke(modDirectory);
        });

        ModDeletedSubscriber = ModDeleted.Subscriber(pluginInterface, modDirectory =>
        {
            ModDirectoriesCache?.Remove(modDirectory);
            ModInfoCache.Remove(modDirectory);
        });

        ModMovedSubscriber = ModMoved.Subscriber(pluginInterface, (oldModDirectory, newModDirectory) =>
        {
            ModDirectoriesCache?.Remove(oldModDirectory);
            ModDirectoriesCache?.Add(newModDirectory);
            ModInfoCache.Remove(oldModDirectory);
        });

        PluginConfigsDirectory = Path.GetFullPath(Path.Combine(pluginInterface.GetPluginConfigDirectory(), ".."));

        // %appdata%\xivlauncher\pluginConfigs\Penumbra\mod_data\{dir}.json
        LocalDataConfigPathTemplate = Path.GetFullPath(Path.Combine(PluginConfigsDirectory, "Penumbra\\mod_data\\{0}.json"));
    }

    public void Dispose()
    {
        ModAddedSubscriber.Dispose();
        ModDeletedSubscriber.Dispose();
        ModMovedSubscriber.Dispose();
    }

    public ImmutableHashSet<string> GetCachedModDirectories()
    {
        ModDirectoriesCache ??= [.. GetModListSubscriber.Invoke().Keys];
        return [.. ModDirectoriesCache];
    }

    public ImmutableHashSet<string> GetCachedModPaths()
    {
        return [.. GetCachedModDirectories().Select(GetCachedModPath)];
    }

    public string GetCachedModPath(string modDirectory)
    {
        if (ModPathsCache.TryGetValue(modDirectory, out var cachedModPath))
        {
            return cachedModPath;
        }

        var (exitCode, modPath, _, __) = GetModPathSubscriber.Invoke(modDirectory);
        if (exitCode != PenumbraApiEc.Success)
        {
            PluginLog.Error($"Failed to retrieve path for mod [{modDirectory}], returning empty");
            return string.Empty;
        }
        ModPathsCache.Add(modDirectory, modPath);

        return modPath;
    }

    public ModInfo GetCachedModInfo(string modDirectory)
    {
        if (ModInfoCache.TryGetValue(modDirectory, out var cachedModInfo))
        {
            return cachedModInfo;
        }

        var modDirectoryPath = Path.Combine(GetModDirectorySubscriber.Invoke(), modDirectory);
        var groupDataPaths = Directory.GetFiles(modDirectoryPath, "group_*.json");

        var modInfo = new ModInfo()
        {
            Directory = modDirectory,
            Path = GetCachedModPath(modDirectory),
            ChangedItems = GetChangedItemsSubscriber.Invoke(modDirectory, string.Empty),
            Data = ParseJsonConfig(string.Format(LocalDataConfigPathTemplate, modDirectory)),
            Default = ParseJsonConfig(Path.Combine(modDirectoryPath, "default_mod.json")),
            Groups = [.. groupDataPaths.Select(ParseJsonConfig)],
            Meta = ParseJsonConfig(Path.Combine(modDirectoryPath, "meta.json"))
        };
        ModInfoCache.Add(modDirectory, modInfo);
        return modInfo;
    }

    public void ClearCaches(string modDirectory)
    {
        ModPathsCache.Remove(modDirectory);
        ModInfoCache.Remove(modDirectory);
    }

    public void ClearCaches()
    {
        ModDirectoriesCache = null;
        ModPathsCache.Clear();
        ModInfoCache.Clear();
    }

    public PenumbraApiEc SetModPath(string modDirectory, string newModPath)
    {
        return SetModPathSubscriber.Invoke(modDirectory, newModPath);
    }

    private Dictionary<string, object?> ParseJsonConfig(string filePath)
    {
        if (!Path.Exists(filePath))
        {
            PluginLog.Error($"Failed to find json config [{filePath}], returning empty");
            return [];
        }
        
        using var reader = new StreamReader(filePath);
        var json = reader.ReadToEnd();

        if (JsonUtils.DeserializeToDynamic(json) is not Dictionary<string, object?> config)
        {
            PluginLog.Error($"Failed to parse json config [{filePath}], returning empty");
            return [];
        }

        return config;
    }
}
