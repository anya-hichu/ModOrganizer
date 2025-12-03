using Dalamud.Plugin.Services;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace ModOrganizer.Json.Files;

public interface IFileBuilder<T>
{
    Parser Parser { get; }
    IPluginLog PluginLog { get; }

    bool TryBuild(JsonElement jsonElement, [NotNullWhen(true)] out T? instance);
}
