using ModOrganizer.Json.Penumbra.SortOrders;
using Penumbra.Api.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Mods;

public interface IModInterop : IDisposable
{
    event Action<string>? OnModAdded;
    event Action<string>? OnModDeleted;
    event Action<string, string>? OnModMoved;

    event Action? OnModsChanged;
    event Action? OnSortOrderChanged;

    void ToggleFsWatchers(bool enable);

    string GetSortOrderPath();
    SortOrder GetSortOrder();

    bool TryGetModInfo(string modDirectory, [NotNullWhen(true)] out ModInfo? modInfo);

    Dictionary<string, string> GetModList();

    string GetModPath(string modDirectory);
    string GetModDirectory(string modPath);
    PenumbraApiEc SetModPath(string modDirectory, string newModPath);

    bool ReloadPenumbra();
}
