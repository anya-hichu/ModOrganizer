using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Loaders;

public static class IFileLoaderExtensions
{
    public static bool TryBuildFromFile<T>(this IFileLoader<T> fileLoader, string path, [NotNullWhen(true)] out T? instance)
    {
        instance = default;

        if (!fileLoader.JsonParser.TryParseFile<JsonElement>(path, out var jsonElement))
        {
            fileLoader.PluginLog.Warning($"Failed to parse {nameof(JsonElement)} from [{path}]");
            return false;
        }

        if (!fileLoader.TryBuild(jsonElement, out instance))
        {
            fileLoader.PluginLog.Debug($"failed to build instance [{typeof(T).Name}] from json file [{path}]");
            return false;
        }

        return true;
    }
}
