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

    public event Action? OnModsChanged;

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
    private FileSystemWatcher SortOrderFileSystemWatcher { get; init; }

    private string DataDirectory { get; init; }
    private FileSystemWatcher DataFileSystemWatcher { get; init; }
    private FileSystemWatcher? DefaultFileSystemWatcher { get; set; }
    private FileSystemWatcher? GroupsFileSystemWatcher { get; set; }
    private FileSystemWatcher? MetaFileSystemWatcher { get; set; }



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
        SortOrderFileSystemWatcher = new FileSystemWatcher(SortOrderDirectory, SORT_ORDER_FILE_NAME)
        {
            InternalBufferSize = INTERNAL_BUFFER_SIZE 
        };
        AddFileSystemListeners(SortOrderFileSystemWatcher, OnSortOrderFileUpdate);

        DataDirectory = Path.GetFullPath(Path.Combine(pluginConfigsDirectory, PENUMBRA_FOLDER_NAME, DATA_FOLDER_NAME));
        DataFileSystemWatcher = new FileSystemWatcher(DataDirectory, DATA_FILE_NAME_PATTERN) 
        {
            InternalBufferSize = INTERNAL_BUFFER_SIZE 
        };
        AddFileSystemListeners(DataFileSystemWatcher, OnDataFileUpdate);

        ModsDirectoryPath = GetModDirectorySubscriber.Invoke();
        CreateModFileSystemWatchers();

        ModDirectoryChangedSubscriber = ModDirectoryChanged.Subscriber(pluginInterface, OnModDirectoryChanged);
    }

    #region Dispose
    public void Dispose()
    {
        SortOrderFileSystemWatcher.Dispose();
        DataFileSystemWatcher.Dispose();
        DisposeModFileSystemWatchers();
        ModAddedSubscriber.Dispose();
        ModDeletedSubscriber.Dispose();
        ModMovedSubscriber.Dispose();
        ModDirectoryChangedSubscriber.Dispose();
    }

    private void DisposeModFileSystemWatchers()
    {

        DefaultFileSystemWatcher?.Dispose();
        GroupsFileSystemWatcher?.Dispose();
        MetaFileSystemWatcher?.Dispose();
        PluginLog.Debug("Disposed mod file system watchers");
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
        InvalidateCaches(modDirectory);
    }

    private void OnModMoved(string modDirectory, string newModDirectory)
    {
        PluginLog.Debug($"Received mod moved event [{modDirectory}] to [{newModDirectory}]");
        InvalidateCaches(modDirectory);
    }

    private void OnModDirectoryChanged(string modDirectoryPath, bool valid)
    {
        if (!valid) return;
        PluginLog.Debug($"Mod directory path changed [{modDirectoryPath}]");
        ModsDirectoryPath = modDirectoryPath;
        DisposeModFileSystemWatchers();
        CreateModFileSystemWatchers();
        InvalidateCaches();
    }
    #endregion

    #region Filesystem
    public void CreateModFileSystemWatchers()
    {
        DefaultFileSystemWatcher = new FileSystemWatcher(ModsDirectoryPath, DEFAULT_FILE_NAME)
        {
            IncludeSubdirectories = true,
            InternalBufferSize = INTERNAL_BUFFER_SIZE
        };
        AddFileSystemListeners(DefaultFileSystemWatcher, OnModFileUpdate);

        GroupsFileSystemWatcher = new FileSystemWatcher(ModsDirectoryPath, GROUP_FILE_NAME_PATTERN)
        {
            IncludeSubdirectories = true,
            InternalBufferSize = INTERNAL_BUFFER_SIZE
        };
        AddFileSystemListeners(GroupsFileSystemWatcher, OnModFileUpdate);

        MetaFileSystemWatcher = new FileSystemWatcher(ModsDirectoryPath, META_FILE_NAME)
        {
            IncludeSubdirectories = true,
            InternalBufferSize = INTERNAL_BUFFER_SIZE
        };
        AddFileSystemListeners(MetaFileSystemWatcher, OnModFileUpdate);
        PluginLog.Debug("Created mod file system watchers");
    }

    private void AddFileSystemListeners(FileSystemWatcher fsWatcher, FileSystemEventHandler? updateHandler)
    {
        fsWatcher.Created += updateHandler;
        fsWatcher.Changed += updateHandler;
        fsWatcher.Deleted += updateHandler;
        fsWatcher.Error += OnFileSystemError;
    }

    public void EnableFileSystemWatchers(bool enable)
    {
        SortOrderFileSystemWatcher.EnableRaisingEvents = enable;
        DataFileSystemWatcher.EnableRaisingEvents = enable;
        DefaultFileSystemWatcher!.EnableRaisingEvents = enable;
        GroupsFileSystemWatcher!.EnableRaisingEvents = enable;
        MetaFileSystemWatcher!.EnableRaisingEvents = enable;
        PluginLog.Debug($"{(enable ? "Enabled" : "Disabled")} raising file system events");
    }

    private void OnSortOrderFileUpdate(object sender, FileSystemEventArgs e)
    {
        PluginLog.Debug($"Sort order config [{e.FullPath}] changed ({e.ChangeType}), invalidating caches");
        InvalidateCaches();
    }

    private void OnDataFileUpdate(object sender, FileSystemEventArgs e)
    {
        var modDirectory = Path.GetFileNameWithoutExtension(e.FullPath);
        PluginLog.Debug($"Data config [{e.FullPath}] changed ({e.ChangeType}), invalidating cache [{modDirectory}]");
        InvalidateCaches(modDirectory);
    }

    private void OnModFileUpdate(object sender, FileSystemEventArgs e)
    {
        var modDirectory = Directory.GetParent(e.FullPath)!.Name;
        PluginLog.Debug($"Mod file config [{e.FullPath}] changed ({e.ChangeType}), invalidating cache [{modDirectory}]");
        InvalidateCaches(modDirectory);
    }

    private void OnFileSystemError(object sender, ErrorEventArgs e)
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
    private void InvalidateCaches()
    {
        PluginLog.Debug($"Invalidate all caches");
        InvalidateSortOrderDataCache();
        InvalidateModInfoCaches();
        OnModsChanged?.Invoke();
    }

    private void InvalidateCaches(string modDirectory)
    {
        PluginLog.Debug($"Invalidate caches [{modDirectory}]");
        InvalidateSortOrderDataCache();
        InvalidateModInfoCache(modDirectory);
        OnModsChanged?.Invoke();
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
            Path = GetModPath(modDirectory),
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
    public Dictionary<string, string> GetModList() => GetModListSubscriber.Invoke();

    public List<ModInfo> GetModInfos() => [.. GetModList().Keys.Select(GetModInfo)];

    public string GetModPath(string modDirectory) => GetSortOrderData().GetValueOrDefault(modDirectory, modDirectory);

    public PenumbraApiEc SetModPath(string modDirectory, string newModPath) {
        var exitCode = SetModPathSubscriber.Invoke(modDirectory, newModPath);
        // Watchers might not be enabled, invalidate manually
        if (exitCode == PenumbraApiEc.Success) InvalidateCaches(modDirectory);
        return exitCode;
    }
    #endregion
}
