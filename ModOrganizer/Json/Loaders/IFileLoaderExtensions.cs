using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Loaders;

public static class IFileLoaderExtensions
{
    public static bool TryBuildFromFile<T>(this IFileLoader<T> loader, string path, [NotNullWhen(true)] out T? instance)
    {
        instance = default;

        if (!loader.JsonParser.TryParseFile<JsonElement>(path, out var jsonElement))
        {
            loader.PluginLog.Warning($"Failed to parse [{nameof(JsonElement)}] from [{path}]");
            return false;
        }

        if (!loader.TryBuild(jsonElement, out instance))
        {
            loader.PluginLog.Debug($"Failed to build instance [{typeof(T).Name}] from json file [{path}]");
            return false;
        }

        return true;
    }
}
