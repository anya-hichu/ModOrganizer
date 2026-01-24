namespace ModOrganizer.Configs.Loaders;

public interface IConfigLoader
{
    IConfig GetOrDefault();
}
