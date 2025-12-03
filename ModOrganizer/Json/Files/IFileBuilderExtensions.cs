using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Files;

public static class IFileBuilderExtensions
{
    public static bool TryBuildFromFile<T>(this IFileBuilder<T> loader, string path, [NotNullWhen(true)] out T? instance) where T : class
    {
        instance = null;

        if (!loader.Parser.TryParseFile<JsonElement>(path, out var jsonElement))
        {
            loader.PluginLog.Warning($"Failed to parse [{nameof(JsonElement)}] from json file [{path}]");
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
