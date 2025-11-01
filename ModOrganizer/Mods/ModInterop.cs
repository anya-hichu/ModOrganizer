using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ModOrganizer.Utils;
using Penumbra.Api.Enums;
using Penumbra.Api.Helpers;
using Penumbra.Api.IpcSubscribers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ModOrganizer.Mods;

public class ModInterop : IDisposable
{
    private static readonly int INTERNAL_BUFFER_SIZE = 1024 * 32;

    private static readonly string PENUMBRA_FOLDER_NAME = "Penumbra";
    private static readonly string SORT_ORDER_FILE_NAME = "sort_order.json";

    private static readonly string DATA_FOLDER_NAME = "mod_data";
    private static readonly string DATA_FILE_NAME_PATTERN = "*.json";

    private static readonly string DEFAULT_FILE_NAME = "default_mod.json";
    private static readonly string GROUP_FILE_NAME_PATTERN = "group_*.json";
    private static readonly string META_FILE_NAME = "meta.json";

    private IPluginLog PluginLog { get; init; }


    public event Action<string>? OnModAdded;

    public event Action? OnModPathsChanged;

    private string ModsDirectoryPath { get; set; }

    private Dictionary<string, ModInfo> ModInfoCaches { get; init; } = [];
    private Dictionary<string, string>? MaybeSortOrderDataCache { get; set; }

    private GetModDirectory GetModDirectorySubscriber { get; init; }
    private GetModList GetModListSubscriber { get; init; }
    private GetChangedItems GetChangedItemsSubscriber { get; init; }
    private SetModPath SetModPathSubscriber { get; init; }

    private EventSubscriber<string, bool> ModDirectoryChangedSubscriber { get; init; }

    private EventSubscriber<string> ModAddedSubscriber { get; init; }
    private EventSubscriber<string> ModDeletedSubscriber { get; init; }
    private EventSubscriber<string, string> ModMovedSubscriber { get; init; }

    private string SortOrderDirectory { get; init; }
    private FileSystemWatcher SortOrderFsWatcher { get; init; }

    private string DataDirectory { get; init; }
    private FileSystemWatcher DataFsWatcher { get; init; }
    private FileSystemWatcher? DefaultFsWatcher { get; set; }
    private FileSystemWatcher? GroupsFsWatcher { get; set; }
    private FileSystemWatcher? MetaFsWatcher { get; set; }



    public ModInterop(IDalamudPluginInterface pluginInterface, IPluginLog pluginLog)
    {
        PluginLog = pluginLog;

        GetModDirectorySubscriber = new(pluginInterface);
        GetModListSubscriber = new(pluginInterface);
        GetChangedItemsSubscriber = new(pluginInterface);
        SetModPathSubscriber = new(pluginInterface);

        ModAddedSubscriber = ModAdded.Subscriber(pluginInterface, OnModAddedInternal);
        ModDeletedSubscriber = ModDeleted.Subscriber(pluginInterface, OnModDeleted);
        ModMovedSubscriber = ModMoved.Subscriber(pluginInterface, OnModMoved);

        var pluginConfigsDirectory = Path.GetFullPath(Path.Combine(pluginInterface.GetPluginConfigDirectory(), ".."));

        SortOrderDirectory = Path.GetFullPath(Path.Combine(pluginConfigsDirectory, PENUMBRA_FOLDER_NAME));
        SortOrderFsWatcher = new FileSystemWatcher(SortOrderDirectory, SORT_ORDER_FILE_NAME)
        {
            InternalBufferSize = INTERNAL_BUFFER_SIZE 
        };
        AttachFsListeners(SortOrderFsWatcher, OnSortOrderFileUpdate);

        DataDirectory = Path.GetFullPath(Path.Combine(pluginConfigsDirectory, PENUMBRA_FOLDER_NAME, DATA_FOLDER_NAME));
        DataFsWatcher = new FileSystemWatcher(DataDirectory, DATA_FILE_NAME_PATTERN) 
        {
            InternalBufferSize = INTERNAL_BUFFER_SIZE 
        };
        AttachFsListeners(DataFsWatcher, OnDataFileUpdate);

        ModsDirectoryPath = GetModDirectorySubscriber.Invoke();
        CreateModFsWatchers();

        ModDirectoryChangedSubscriber = ModDirectoryChanged.Subscriber(pluginInterface, OnModDirectoryChanged);
    }

    #region Dispose
    public void Dispose()
    {
        SortOrderFsWatcher.Dispose();
        DataFsWatcher.Dispose();
        DisposeModFsWatchers();
        ModAddedSubscriber.Dispose();
        ModDeletedSubscriber.Dispose();
        ModMovedSubscriber.Dispose();
        ModDirectoryChangedSubscriber.Dispose();
    }

    private void DisposeModFsWatchers()
    {

        DefaultFsWatcher?.Dispose();
        GroupsFsWatcher?.Dispose();
        MetaFsWatcher?.Dispose();
        PluginLog.Debug("Disposed mod FS watchers");
    }
    #endregion

    #region Subscribers
    private void OnModAddedInternal(string modDirectory)
    {
        PluginLog.Debug($"Received mod added event [{modDirectory}]");
        // Watchers might not be enabled, invalidate manually
        InvalidateCaches(modDirectory);
        OnModAdded?.Invoke(modDirectory);
    }

    private void OnModDeleted(string modDirectory)
    {
        PluginLog.Debug($"Received mod deleted event [{modDirectory}]");
        InvalidateModInfoCache(modDirectory);
    }

    private void OnModMoved(string modDirectory, string newModDirectory)
    {
        PluginLog.Debug($"Received mod moved event [{modDirectory}] to [{newModDirectory}]");
        InvalidateModInfoCache(modDirectory);
    }

    private void OnModDirectoryChanged(string modDirectoryPath, bool valid)
    {
        if (!valid) return;
        PluginLog.Debug($"Mod directory path changed [{modDirectoryPath}]");
        ModsDirectoryPath = modDirectoryPath;
        DisposeModFsWatchers();
        CreateModFsWatchers();
    }
    #endregion

    #region Filesystem
    public void CreateModFsWatchers()
    {
        DefaultFsWatcher = new FileSystemWatcher(ModsDirectoryPath, DEFAULT_FILE_NAME)
        {
            IncludeSubdirectories = true,
            InternalBufferSize = INTERNAL_BUFFER_SIZE
        };
        AttachFsListeners(DefaultFsWatcher, OnModFileUpdate);

        GroupsFsWatcher = new FileSystemWatcher(ModsDirectoryPath, GROUP_FILE_NAME_PATTERN)
        {
            IncludeSubdirectories = true,
            InternalBufferSize = INTERNAL_BUFFER_SIZE
        };
        AttachFsListeners(GroupsFsWatcher, OnModFileUpdate);

        MetaFsWatcher = new FileSystemWatcher(ModsDirectoryPath, META_FILE_NAME)
        {
            IncludeSubdirectories = true,
            InternalBufferSize = INTERNAL_BUFFER_SIZE
        };
        AttachFsListeners(MetaFsWatcher, OnModFileUpdate);
        PluginLog.Debug("Created mod FS watchers");
    }

    private void AttachFsListeners(FileSystemWatcher fsWatcher, FileSystemEventHandler? updateHandler)
    {
        fsWatcher.Created += updateHandler;
        fsWatcher.Changed += updateHandler;
        fsWatcher.Deleted += updateHandler;
        fsWatcher.Error += OnFsWatcherError;
    }

    public void EnableRaisingFsEvents(bool enable)
    {
        SortOrderFsWatcher.EnableRaisingEvents = enable;
        DataFsWatcher.EnableRaisingEvents = enable;
        DefaultFsWatcher!.EnableRaisingEvents = enable;
        GroupsFsWatcher!.EnableRaisingEvents = enable;
        MetaFsWatcher!.EnableRaisingEvents = enable;
        PluginLog.Debug($"{(enable ? "Enabled" : "Disabled")} raising FS events");
    }

    private void OnSortOrderFileUpdate(object sender, FileSystemEventArgs e)
    {
        PluginLog.Debug($"Sort order config [{e.FullPath}] changed ({e.ChangeType}), invalidating caches");
        InvalidateSortOrderDataCache();
        InvalidateModInfoCaches();
    }

    private void OnDataFileUpdate(object sender, FileSystemEventArgs e)
    {
        var modDirectory = Path.GetFileNameWithoutExtension(e.FullPath);
        PluginLog.Debug($"Data config [{e.FullPath}] changed ({e.ChangeType}), invalidating cache [{modDirectory}]");
        InvalidateModInfoCache(modDirectory);
    }

    private void OnModFileUpdate(object sender, FileSystemEventArgs e)
    {
        var modDirectory = Directory.GetParent(e.FullPath)!.Name;
        PluginLog.Debug($"Mod file config [{e.FullPath}] changed ({e.ChangeType}), invalidating cache [{modDirectory}]");
        InvalidateModInfoCache(modDirectory);
    }

    private void OnFsWatcherError(object sender, ErrorEventArgs e)
    {
        PluginLog.Debug($"Watcher [{sender.GetHashCode()}] returned error ({e?.GetException().Message}), ignoring");
    }

    private Dictionary<string, object?> ParseJsonFile(string filePath)
    {
        if (!Path.Exists(filePath))
        {
            PluginLog.Debug($"Failed to find json config [{filePath}], returning empty");
            return [];
        }

        using var reader = new StreamReader(filePath);
        var json = reader.ReadToEnd();

        try
        {
            if (JsonUtils.DeserializeToDynamic(json) is not Dictionary<string, object?> config)
            {
                PluginLog.Debug($"Failed to parse json config [{filePath}], returning empty");
                return [];
            }

            return config;
        }
        catch (JsonException e)
        {
            PluginLog.Debug($"Failed to parse json config [{filePath}] ({e.Message}), returning empty");
            return [];
        }
    }
    #endregion

    #region Cache
    private void InvalidateCaches(string modDirectory)
    {
        PluginLog.Debug($"Invalidate caches [{modDirectory}]");
        InvalidateSortOrderDataCache();
        InvalidateModInfoCache(modDirectory);
    }

    private void InvalidateModInfoCaches()
    {
        PluginLog.Debug($"Invalidated mod info caches (count: {ModInfoCaches.Count})");
        ModInfoCaches.Clear();
    }

    private void InvalidateModInfoCache(string modDirectory) 
    {
        PluginLog.Debug($"Invalidated mod info cache [{modDirectory}]");
        ModInfoCaches.Remove(modDirectory);
    }

    private void InvalidateSortOrderDataCache()
    {
        PluginLog.Debug($"Invalidated sort order data cache [count: {MaybeSortOrderDataCache?.Count}]");
        MaybeSortOrderDataCache = null;
        OnModPathsChanged?.Invoke();
    }

    private Dictionary<string, string> GetSortOrderData()
    {
        if (MaybeSortOrderDataCache != null) return MaybeSortOrderDataCache;
        var sortOrder = ParseJsonFile(Path.Combine(SortOrderDirectory, SORT_ORDER_FILE_NAME));
        MaybeSortOrderDataCache = sortOrder.GetValueOrDefault("Data") is Dictionary<string, object?> data ? data.ToDictionary(e => e.Key, e => e.Value == null ? e.Key : e.Value.ToString()!) : [];
        PluginLog.Debug($"Loaded sort order data cache (count: {MaybeSortOrderDataCache!.Count})");
        return MaybeSortOrderDataCache;
    }

    public ModInfo GetModInfo(string modDirectory)
    {
        if (ModInfoCaches.TryGetValue(modDirectory, out var cache)) return cache;

        var modInfo = new ModInfo()
        {
            Directory = modDirectory,
            Path = GetSortOrderData().GetValueOrDefault(modDirectory, modDirectory),
            ChangedItems = GetChangedItemsSubscriber.Invoke(modDirectory, string.Empty),
            Data = ParseJsonFile(Path.Combine(DataDirectory, $"{modDirectory}.json")),

            Default = ParseJsonFile(Path.Combine(ModsDirectoryPath, modDirectory, DEFAULT_FILE_NAME)),
            Groups = [.. Directory.GetFiles(Path.Combine(ModsDirectoryPath, modDirectory), GROUP_FILE_NAME_PATTERN).Select(ParseJsonFile)],
            Meta = ParseJsonFile(Path.Combine(ModsDirectoryPath, modDirectory, META_FILE_NAME)),
        };

        ModInfoCaches.Add(modDirectory, modInfo);
        return modInfo;
    }
    #endregion

    #region API

    private Dictionary<string, string> GetModList() => GetModListSubscriber.Invoke();

    public List<ModInfo> GetModInfos() => [.. GetModList().Keys.Select(GetModInfo)];

    public Dictionary<string, string> GetModPaths() => GetModList().Keys.ToDictionary(e => e, e => GetSortOrderData().GetValueOrDefault(e, e));

    public PenumbraApiEc SetModPath(string modDirectory, string newModPath) {
        var exitCode = SetModPathSubscriber.Invoke(modDirectory, newModPath);
        if (exitCode == PenumbraApiEc.Success)
        {
            // Watchers might not be enabled, invalidate manually
            InvalidateCaches(modDirectory);
        }
        return exitCode;
    }
    #endregion
}
