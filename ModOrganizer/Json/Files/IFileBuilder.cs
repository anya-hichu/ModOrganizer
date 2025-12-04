using Dalamud.Plugin.Services;

namespace ModOrganizer.Json.Files;

public interface IFileBuilder<T> : IBuilder<T> where T : class
{
    FileParser FileParser { get; }
    IPluginLog PluginLog { get; }
}
