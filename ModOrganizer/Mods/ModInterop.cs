using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ModOrganizer.Json.Penumbra.DefaultMods;
using ModOrganizer.Json.Penumbra.Groups;
using ModOrganizer.Json.Penumbra.LocalModDatas;
using ModOrganizer.Json.Penumbra.ModMetas;
using ModOrganizer.Json.Penumbra.SortOrders;
using ModOrganizer.Json.Readers.Files;
using Penumbra.Api.Enums;
using Penumbra.Api.Helpers;
using Penumbra.Api.IpcSubscribers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace ModOrganizer.Mods;

public class ModInterop : IModInterop
{
    private static readonly int INTERNAL_BUFFER_SIZE = 1024 * 32;

    public static readonly string SORT_ORDER_FILE_NAME = "sort_order.json";

    private static readonly string DATA_FOLDER_NAME = "mod_data";
    private static readonly string DATA_FILE_NAME_PATTERN = "*.json";

    private static readonly string DEFAULT_FILE_NAME = "default_mod.json";
    private static readonly string GROUP_FILE_NAME_PATTERN = "group_*.json";
    private static readonly string META_FILE_NAME = "meta.json";

    public static readonly string RELOAD_PENUMBRA_COMMAND = "/penumbra reload";

    private ICommandManager CommandManager { get; init; }
    private IPluginLog PluginLog { get; init; }

    private IDefaultModReader DefaultModReader { get; init; }
    private IGroupGenericReader GroupGenericReader { get; init; }
    private IModMetaReader ModMetaReader { get; init; }
    private ILocalModDataReader LocalModDataReader { get; init; }
    private ISortOrderReader SortOrderReader { get; init; }

    public event Action<string>? OnModAdded;
    public event Action<string>? OnModDeleted;
    public event Action<string, string>? OnModMoved;

    public event Action? OnModsChanged;
    public event Action? OnSortOrderChanged;

    private string ModsDirectoryPath { get; set; }

    private Dictionary<string, ModInfo?> ModInfoCaches { get; init; } = [];
    private SortOrder? MaybeSortOrderCache { get; set; }

    private GetModDirectory GetModDirectorySubscriber { get; init; }
    private GetModList GetModListSubscriber { get; init; }
    private GetChangedItems GetChangedItemsSubscriber { get; init; }
    private SetModPath SetModPathSubscriber { get; init; }

    private EventSubscriber<string, bool> ModDirectoryChangedSubscriber { get; init; }

    private EventSubscriber<string> ModAddedSubscriber { get; init; }
    private EventSubscriber<string> ModDeletedSubscriber { get; init; }
    private EventSubscriber<string, string> ModMovedSubscriber { get; init; }

    private string PenumbraConfigDirectory { get; init; }
    private FileSystemWatcher SortOrderFileSystemWatcher { get; init; }

    private string DataDirectory { get; init; }
    private FileSystemWatcher DataFileSystemWatcher { get; init; }
    private FileSystemWatcher? DefaultFileSystemWatcher { get; set; }
    private FileSystemWatcher? GroupsFileSystemWatcher { get; set; }
    private FileSystemWatcher? MetaFileSystemWatcher { get; set; }

    public ModInterop(ICommandManager commandManager, IDefaultModReader defaultModReader, IGroupGenericReader groupGenericReader, ILocalModDataReader localModDataReader, 
        IModMetaReader modMetaReader, IDalamudPluginInterface pluginInterface, IPluginLog pluginLog, ISortOrderReader sortOrderReader)
    {
        CommandManager = commandManager;
        DefaultModReader = defaultModReader;
        GroupGenericReader = groupGenericReader;
        LocalModDataReader = localModDataReader;
        ModMetaReader = modMetaReader;
        SortOrderReader = sortOrderReader;

        PluginLog = pluginLog;

        GetModDirectorySubscriber = new(pluginInterface);
        GetModListSubscriber = new(pluginInterface);
        GetChangedItemsSubscriber = new(pluginInterface);
        SetModPathSubscriber = new(pluginInterface);

        ModAddedSubscriber = ModAdded.Subscriber(pluginInterface, OnWrappedModAdded);
        ModDeletedSubscriber = ModDeleted.Subscriber(pluginInterface, OnWrappedModDeleted);
        ModMovedSubscriber = ModMoved.Subscriber(pluginInterface, OnWrappedModMoved);

        PenumbraConfigDirectory = Path.Combine(pluginInterface.ConfigDirectory.Parent!.FullName, nameof(Penumbra));
        SortOrderFileSystemWatcher = new FileSystemWatcher(PenumbraConfigDirectory, SORT_ORDER_FILE_NAME) { InternalBufferSize = INTERNAL_BUFFER_SIZE };
        AddFsEventHandlers(SortOrderFileSystemWatcher, OnSortOrderFileUpdate);

        DataDirectory = Path.Combine(PenumbraConfigDirectory, DATA_FOLDER_NAME);
        DataFileSystemWatcher = new FileSystemWatcher(DataDirectory, DATA_FILE_NAME_PATTERN) { InternalBufferSize = INTERNAL_BUFFER_SIZE };
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
        PluginLog.Debug($"Received mod directory changed [{modDirectoryPath}]");
        ModsDirectoryPath = modDirectoryPath;
        DisposeModFsWatchers();
        CreateModFsWatchers();
        InvalidateCaches();
    }

    #endregion

    #region Watchers

    private void CreateModFsWatchers()
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
        PluginLog.Debug($"{(enable ? "Enabled" : "Disabled")} file system watchers");
    }

    private void OnSortOrderFileUpdate(object sender, FileSystemEventArgs e)
    {
        PluginLog.Debug($"Invalidating caches after sort order config file [{e.FullPath}] changed ({e.ChangeType})");
        InvalidateCaches();
    }

    private void OnDataFileUpdate(object sender, FileSystemEventArgs e)
    {
        var modDirectory = Path.GetFileNameWithoutExtension(e.FullPath);
        PluginLog.Debug($"Invalidating cache [{modDirectory}] after data config file [{e.FullPath}] changed ({e.ChangeType})");
        InvalidateCaches(modDirectory);
    }

    private void OnModFileUpdate(object sender, FileSystemEventArgs e)
    {
        var modDirectory = Directory.GetParent(e.FullPath)!.Name;
        PluginLog.Debug($"Invalidating cache [{modDirectory}] after mod config file [{e.FullPath}] changed ({e.ChangeType})");
        InvalidateCaches(modDirectory);
    }

    private void OnFsWatcherError(object sender, ErrorEventArgs e) => PluginLog.Debug($"Ignoring file system watcher [{sender.GetHashCode()}] error ({e.GetException().Message})");

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
        PluginLog.Debug($"Invalidating sort order data cache");
        MaybeSortOrderCache = null;
    }

    public string GetSortOrderPath() => Path.Combine(PenumbraConfigDirectory, SORT_ORDER_FILE_NAME);

    public SortOrder GetSortOrder()
    {
        if (MaybeSortOrderCache != null) return MaybeSortOrderCache;

        if (SortOrderReader.TryReadFromFile(GetSortOrderPath(), out var sortOrder))
        {
            MaybeSortOrderCache = sortOrder;
            PluginLog.Debug($"Loaded [{nameof(SortOrder)}] cache (count: {sortOrder.Data.Count})");
        }
        else
        {
            MaybeSortOrderCache = new();
            PluginLog.Warning($"Failed to parse [{nameof(SortOrder)}], cached empty until next file system update or reload");
        }
        OnSortOrderChanged?.Invoke();
        return MaybeSortOrderCache;
    }

    #endregion

    #region API

    public bool TryGetModInfo(string modDirectory, [NotNullWhen(true)] out ModInfo? modInfo)
    {
        if (ModInfoCaches.TryGetValue(modDirectory, out modInfo)) return modInfo != null;

        var modDirectoryPath = Path.Combine(ModsDirectoryPath, modDirectory);

        if (!LocalModDataReader.TryReadFromFile(Path.Combine(DataDirectory, $"{modDirectory}.json"), out var localModData)) PluginLog.Warning($"Failed to build [{nameof(LocalModDataV3)}] for mod [{modDirectory}]");
        if (!DefaultModReader.TryReadFromFile(Path.Combine(modDirectoryPath, DEFAULT_FILE_NAME), out var defaultMod)) PluginLog.Warning($"Failed to build [{nameof(DefaultMod)}] for mod [{modDirectory}]");
        if (!ModMetaReader.TryReadFromFile(Path.Combine(modDirectoryPath, META_FILE_NAME), out var modMeta)) PluginLog.Warning($"Failed to build [{nameof(ModMetaV3)}] for mod [{modDirectory}]");

        var groupFilePaths = Directory.Exists(modDirectoryPath) ? Directory.GetFiles(modDirectoryPath, GROUP_FILE_NAME_PATTERN) : [];
        var maybeGroups = groupFilePaths.Select(p => {
            if (!GroupGenericReader.TryReadFromFile(p, out var group)) PluginLog.Warning($"Failed to build [{nameof(Group)}] for mod [{modDirectory}]");
            return group;
        }).ToArray();

        var groups = maybeGroups.OfType<Group>().ToArray();
        if (localModData == null || defaultMod == null || modMeta == null || maybeGroups.Length != groups.Length)
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

    public string GetModPath(string modDirectory) => GetSortOrder().Data.GetValueOrDefault(modDirectory, modDirectory);
    public string GetModDirectory(string modPath) => GetSortOrder().Data.FirstOrDefault(d => d.Value == modPath, new(modPath, modPath)).Key;

    public PenumbraApiEc SetModPath(string modDirectory, string newModPath)
    {
        var exitCode = SetModPathSubscriber.Invoke(modDirectory, newModPath);
        if (exitCode == PenumbraApiEc.Success)
        {
            PluginLog.Info($"Set mod [{modDirectory}] path to [{newModPath}]");
            // Watchers might not be enabled, invalidate manually
            InvalidateCaches(modDirectory);
            return exitCode;
        }
        PluginLog.Error($"Failed to set mod [{modDirectory}] path to [{newModPath}] ({exitCode})");
        return exitCode;
    }

    public bool ReloadPenumbra() => CommandManager.ProcessCommand(RELOAD_PENUMBRA_COMMAND);

    #endregion
}
