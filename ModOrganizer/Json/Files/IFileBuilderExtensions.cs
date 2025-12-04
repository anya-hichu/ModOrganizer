using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Files;

public static class IFileBuilderExtensions
{
    public static bool TryBuildFromFile<T>(this IFileBuilder<T> fileBuilder, string path, [NotNullWhen(true)] out T? instance) where T : class
    {
        instance = null;

        if (!fileBuilder.FileParser.TryParseFile<JsonElement>(path, out var jsonElement))
        {
            fileBuilder.PluginLog.Warning($"Failed to parse [{nameof(JsonElement)}] from json file [{path}]");
            return false;
        }

        if (!fileBuilder.TryBuild(jsonElement, out instance))
        {
            fileBuilder.PluginLog.Debug($"Failed to build instance [{typeof(T).Name}] from json file [{path}]");
            return false;
        }

        return true;
    }
}
