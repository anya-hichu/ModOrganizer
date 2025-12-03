using Dalamud.Plugin.Services;

namespace ModOrganizer.Json.Files;

public interface IFileBuilder<T> : IBuilder<T> where T : class
{
    Parser Parser { get; }
    IPluginLog PluginLog { get; }
}
