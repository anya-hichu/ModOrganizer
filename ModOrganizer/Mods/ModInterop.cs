using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ModOrganizer.Json;
using ModOrganizer.Json.DefaultMods;
using ModOrganizer.Json.Groups;
using ModOrganizer.Json.LocalModData;
using ModOrganizer.Json.ModMetas;
using ModOrganizer.Json.Files;
using ModOrganizer.Json.SortOrders;
using Penumbra.Api.Enums;
using Penumbra.Api.Helpers;
using Penumbra.Api.IpcSubscribers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

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

    private DefaultModBuilder DefaultModBuilder { get; init; }
    private GroupFactory GroupFactory { get; init; }
    private Parser Parser { get; init; }
    private LocalModDataBuilder LocalModDataBuilder { get; init; }
    private IPluginLog PluginLog { get; init; }
    private ModMetaBuilder ModMetaBuilder { get; init; }


    public event Action<string>? OnModAdded;
    public event Action<string>? OnModDeleted;
    public event Action<string, string>? OnModMoved;

    public event Action? OnModsChanged;

    private string ModsDirectoryPath { get; set; }

    private Dictionary<string, ModInfo?> ModInfoCaches { get; init; } = [];
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
        DefaultModBuilder = new(pluginLog);
        GroupFactory = new(pluginLog);
        Parser = new(pluginLog);
        LocalModDataBuilder = new(pluginLog);
        ModMetaBuilder = new(pluginLog);
        PluginLog = pluginLog;

        GetModDirectorySubscriber = new(pluginInterface);
        GetModListSubscriber = new(pluginInterface);
        GetChangedItemsSubscriber = new(pluginInterface);
        SetModPathSubscriber = new(pluginInterface);

        ModAddedSubscriber = ModAdded.Subscriber(pluginInterface, OnWrappedModAdded);
        ModDeletedSubscriber = ModDeleted.Subscriber(pluginInterface, OnWrappedModDeleted);
        ModMovedSubscriber = ModMoved.Subscriber(pluginInterface, OnWrappedModMoved);

        var pluginConfigsDirectory = Path.GetFullPath(Path.Combine(pluginInterface.GetPluginConfigDirectory(), ".."));

        SortOrderDirectory = Path.GetFullPath(Path.Combine(pluginConfigsDirectory, PENUMBRA_FOLDER_NAME));
        SortOrderFileSystemWatcher = new FileSystemWatcher(SortOrderDirectory, SORT_ORDER_FILE_NAME)
        {
            InternalBufferSize = INTERNAL_BUFFER_SIZE 
        };
        AddFsEventHandlers(SortOrderFileSystemWatcher, OnSortOrderFileUpdate);

        DataDirectory = Path.GetFullPath(Path.Combine(pluginConfigsDirectory, PENUMBRA_FOLDER_NAME, DATA_FOLDER_NAME));
        DataFileSystemWatcher = new FileSystemWatcher(DataDirectory, DATA_FILE_NAME_PATTERN) 
        {
            InternalBufferSize = INTERNAL_BUFFER_SIZE 
        };
        AddFsEventHandlers(DataFileSystemWatcher, OnDataFileUpdate);

        ModsDirectoryPath = GetModDirectorySubscriber.Invoke();
        CreateModFsWatchers();

        ModDirectoryChangedSubscriber = ModDirectoryChanged.Subscriber(pluginInterface, OnModDirectoryChanged);
    }

    #region Dispose

    public void Dispose()
    {
        SortOrderFileSystemWatcher.Dispose();
        DataFileSystemWatcher.Dispose();
        DisposeModFsWatchers();
        ModAddedSubscriber.Dispose();
        ModDeletedSubscriber.Dispose();
        ModMovedSubscriber.Dispose();
        ModDirectoryChangedSubscriber.Dispose();
    }

    private void DisposeModFsWatchers()
    {

        DefaultFileSystemWatcher?.Dispose();
        GroupsFileSystemWatcher?.Dispose();
        MetaFileSystemWatcher?.Dispose();
        PluginLog.Debug("Disposed mod file system watchers");
    }

    #endregion

    #region Listeners

    private void OnWrappedModAdded(string modDirectory)
    {
        PluginLog.Debug($"Received mod added event [{modDirectory}]");
        // Watchers might not be enabled, invalidate manually
        InvalidateCaches(modDirectory);
        OnModAdded?.Invoke(modDirectory);
    }

    private void OnWrappedModDeleted(string modDirectory)
    {
        PluginLog.Debug($"Received mod deleted event [{modDirectory}]");
        InvalidateCaches(modDirectory);
        OnModDeleted?.Invoke(modDirectory);
    }

    private void OnWrappedModMoved(string modDirectory, string newModDirectory)
    {
        PluginLog.Debug($"Received mod moved event [{modDirectory}] to [{newModDirectory}]");
        InvalidateCaches(modDirectory);
        OnModMoved?.Invoke(modDirectory, newModDirectory);
    }

    private void OnModDirectoryChanged(string modDirectoryPath, bool valid)
    {
        if (!valid) return;
        PluginLog.Debug($"Mod directory path changed [{modDirectoryPath}]");
        ModsDirectoryPath = modDirectoryPath;
        DisposeModFsWatchers();
        CreateModFsWatchers();
        InvalidateCaches();
    }

    #endregion

    #region Watchers

    public void CreateModFsWatchers()
    {
        DefaultFileSystemWatcher = new FileSystemWatcher(ModsDirectoryPath, DEFAULT_FILE_NAME)
        {
            IncludeSubdirectories = true,
            InternalBufferSize = INTERNAL_BUFFER_SIZE
        };
        AddFsEventHandlers(DefaultFileSystemWatcher, OnModFileUpdate);

        GroupsFileSystemWatcher = new FileSystemWatcher(ModsDirectoryPath, GROUP_FILE_NAME_PATTERN)
        {
            IncludeSubdirectories = true,
            InternalBufferSize = INTERNAL_BUFFER_SIZE
        };
        AddFsEventHandlers(GroupsFileSystemWatcher, OnModFileUpdate);

        MetaFileSystemWatcher = new FileSystemWatcher(ModsDirectoryPath, META_FILE_NAME)
        {
            IncludeSubdirectories = true,
            InternalBufferSize = INTERNAL_BUFFER_SIZE
        };
        AddFsEventHandlers(MetaFileSystemWatcher, OnModFileUpdate);
        PluginLog.Debug("Created mod file system watchers");
    }

    private void AddFsEventHandlers(FileSystemWatcher fsWatcher, FileSystemEventHandler? fsEventHandler)
    {
        fsWatcher.Created += fsEventHandler;
        fsWatcher.Changed += fsEventHandler;
        fsWatcher.Deleted += fsEventHandler;
        fsWatcher.Error += OnFsWatcherError;
    }

    public void ToggleFsWatchers(bool enable)
    {
        SortOrderFileSystemWatcher.EnableRaisingEvents = enable;
        DataFileSystemWatcher.EnableRaisingEvents = enable;
        DefaultFileSystemWatcher!.EnableRaisingEvents = enable;
        GroupsFileSystemWatcher!.EnableRaisingEvents = enable;
        MetaFileSystemWatcher!.EnableRaisingEvents = enable;
        PluginLog.Debug($"Toggled file system watchers [{enable}]");
    }

    private void OnSortOrderFileUpdate(object sender, FileSystemEventArgs e)
    {
        PluginLog.Debug($"Sort order config file [{e.FullPath}] changed ({e.ChangeType}), invalidating caches");
        InvalidateCaches();
    }

    private void OnDataFileUpdate(object sender, FileSystemEventArgs e)
    {
        var modDirectory = Path.GetFileNameWithoutExtension(e.FullPath);
        PluginLog.Debug($"Data config file [{e.FullPath}] changed ({e.ChangeType}), invalidating cache [{modDirectory}]");
        InvalidateCaches(modDirectory);
    }

    private void OnModFileUpdate(object sender, FileSystemEventArgs e)
    {
        var modDirectory = Directory.GetParent(e.FullPath)!.Name;
        PluginLog.Debug($"Mod config file [{e.FullPath}] changed ({e.ChangeType}), invalidating cache [{modDirectory}]");
        InvalidateCaches(modDirectory);
    }

    private void OnFsWatcherError(object sender, ErrorEventArgs e) => PluginLog.Debug($"Watcher [{sender.GetHashCode()}] returned error ({e?.GetException().Message}), ignoring");

    #endregion

    #region Cache

    private void InvalidateCaches()
    {
        PluginLog.Debug($"Invalidating all caches");
        InvalidateSortOrderDataCache();
        InvalidateModInfoCaches();
        OnModsChanged?.Invoke();
    }

    private void InvalidateCaches(string modDirectory)
    {
        PluginLog.Debug($"Invalidating caches for mod [{modDirectory}]");
        InvalidateSortOrderDataCache();
        InvalidateModInfoCache(modDirectory);
        OnModsChanged?.Invoke();
    }

    private void InvalidateModInfoCaches()
    {
        PluginLog.Debug($"Invalidating mod info caches (count: {ModInfoCaches.Count})");
        ModInfoCaches.Clear();
    }

    private void InvalidateModInfoCache(string modDirectory) 
    {
        PluginLog.Debug($"Invalidating mod info cache [{modDirectory}]");
        ModInfoCaches.Remove(modDirectory);
    }

    private void InvalidateSortOrderDataCache()
    {
        PluginLog.Debug($"Invalidating sort order data cache [count: {MaybeSortOrderDataCache?.Count}]");
        MaybeSortOrderDataCache = null;
    }

    private Dictionary<string, string> GetSortOrderData()
    {
        if (MaybeSortOrderDataCache != null) return MaybeSortOrderDataCache;

        if (Parser.TryParseFile<SortOrder>(Path.Combine(SortOrderDirectory, SORT_ORDER_FILE_NAME), out var sortOrder))
        {
            MaybeSortOrderDataCache = sortOrder.Data;
            PluginLog.Debug($"Loaded [{nameof(SortOrder)}] cache (count: {MaybeSortOrderDataCache.Count})");
        }
        else
        {
            MaybeSortOrderDataCache = [];
            PluginLog.Warning($"Failed to parse [{nameof(SortOrder)}], cached empty until next file system update or reload");
        }

        return MaybeSortOrderDataCache;
    }

    #endregion

    #region API

    public bool TryGetModInfo(string modDirectory, [NotNullWhen(true)] out ModInfo? modInfo)
    {
        if (ModInfoCaches.TryGetValue(modDirectory, out modInfo)) return modInfo != null;

        if (!LocalModDataBuilder.TryBuildFromFile(Path.Combine(DataDirectory, $"{modDirectory}.json"), out var localModData)) PluginLog.Debug($"Failed to build [{nameof(LocalModData)}] for mod [{modDirectory}]");
        if (!DefaultModBuilder.TryBuildFromFile(Path.Combine(ModsDirectoryPath, modDirectory, DEFAULT_FILE_NAME), out var defaultMod)) PluginLog.Debug($"Failed to build [{nameof(DefaultMod)}] for mod [{modDirectory}]");
        if (!ModMetaBuilder.TryBuildFromFile(Path.Combine(ModsDirectoryPath, modDirectory, META_FILE_NAME), out var modMeta)) PluginLog.Debug($"Failed to build [{nameof(ModMeta)}] for mod [{modDirectory}]");

        var maybeGroups = Directory.GetFiles(Path.Combine(ModsDirectoryPath, modDirectory), GROUP_FILE_NAME_PATTERN).Select(p => {
            if (!GroupFactory.TryBuildFromFile(p, out var group)) PluginLog.Debug($"Failed to build [{nameof(Group)}] for mod [{modDirectory}]");
            return group;
        }).ToList();

        var groups = maybeGroups.OfType<Group>().ToList();
        if (localModData == null || defaultMod == null || modMeta == null || maybeGroups.Count != groups.Count)
        {
            ModInfoCaches.Add(modDirectory, null);
            PluginLog.Warning($"Failed to build [{nameof(ModInfo)}] for mod [{modDirectory}], cached failure until next file system update or reload");
            return false;
        }

        modInfo = new()
        {
            Directory = modDirectory,
            Path = GetModPath(modDirectory),
            ChangedItems = GetChangedItemsSubscriber.Invoke(modDirectory, string.Empty),
            Data = localModData,
            Default = defaultMod,
            Meta = modMeta,
            Groups = groups
        };

        ModInfoCaches.Add(modDirectory, modInfo);

        return true;
    }

    public Dictionary<string, string> GetModList() => GetModListSubscriber.Invoke();

    public string GetModPath(string modDirectory) => GetSortOrderData().GetValueOrDefault(modDirectory, modDirectory);

    public PenumbraApiEc SetModPath(string modDirectory, string newModPath) {
        var exitCode = SetModPathSubscriber.Invoke(modDirectory, newModPath);
        // Watchers might not be enabled, invalidate manually
        if (exitCode == PenumbraApiEc.Success) InvalidateCaches(modDirectory);
        return exitCode;
    }
    #endregion
}
