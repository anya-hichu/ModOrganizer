using Dalamud.Plugin.Services;
using ModOrganizer.Json.Parsers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Loaders;

public interface IFileParser<T>
{
    JsonParser JsonParser { get; }
    IPluginLog PluginLog { get; }

    bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out T? instance);
}
