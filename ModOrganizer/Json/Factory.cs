using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json;

public abstract class Factory<T>(IPluginLog pluginLog) : Builder<T>(pluginLog) where T : class
{
    protected abstract bool TryGetBuilder(JsonElement jsonElement, [NotNullWhen(true)] out Builder<T>? builder);

    public override bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out T? instance)
    {
        instance = null;

        if (!TryGetBuilder(jsonElement, out var builder))
        {
            PluginLog.Debug($"Failed to find [{typeof(T).Name}] builder for [{typeof(T).Name}]:\n{jsonElement}");
            return false;
        }

        if (!builder.TryBuild(jsonElement, out instance))
        {
            PluginLog.Debug($"Failed to build instance [{typeof(T).Name}] for [{typeof(T).Name}] using builder [{builder.GetType().Name}]:\n{jsonElement}");
            return false;
        }

        return true;
    }
}
