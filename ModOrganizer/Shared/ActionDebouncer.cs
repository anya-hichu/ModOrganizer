using Dalamud.Plugin.Services;
using System;
using System.Collections.Generic;
using ThrottleDebounce;

namespace ModOrganizer.Shared;

public class ActionDebouncer(IPluginLog pluginLog)
{
    private IPluginLog PluginLog { get; init; } = pluginLog;

    private Dictionary<string, RateLimitedAction> DebouncedActions { get; init; } = [];

    public void Invoke(string key, Action action, TimeSpan wait, bool leading = false, bool trailing = true)
    {
        if (!DebouncedActions.TryGetValue(key, out var debouncedAction)) DebouncedActions[key] = debouncedAction = Debouncer.Debounce(WrapAction(key, action), wait, leading, trailing);
        
        debouncedAction.Invoke();
    }

    private Action WrapAction(string key, Action action) => () =>
    {
        PluginLog.Verbose($"Executing debounced action [{key}]");
        try
        {
            action();
        }
        finally
        {
            DebouncedActions.Remove(key);
        }
    };

    public void Dispose()
    {
        foreach (var debouncedAction in DebouncedActions.Values) debouncedAction.Dispose();
    }
}
