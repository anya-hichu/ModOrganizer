namespace ModOrganizer.Configs;

public interface IConfigLoader
{
    IConfig GetOrDefault();
}
