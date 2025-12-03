using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Loaders;

public static class IFileParserExtensions
{
    public static bool TryBuildFromFile<T>(this IFileParser<T> loader, string path, [NotNullWhen(true)] out T? instance) where T : class
    {
        instance = null;

        if (!loader.JsonParser.TryParseFile<JsonElement>(path, out var jsonElement))
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
