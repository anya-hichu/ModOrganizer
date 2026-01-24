namespace ModOrganizer.Configs.Mergers;

public interface IConfigMerger
{
    int CountConflicts(IConfig newConfig);

    void Merge(IConfig newConfig, bool overwrite);
}
