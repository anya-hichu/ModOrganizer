using ModOrganizer.Rules;
using System.Collections.Generic;

namespace ModOrganizer.Configs.Mergers;

public interface IConfigMerger
{
    IEnumerable<Rule> GetConflicts(IConfig otherConfig);

    void Merge(IConfig otherConfig, bool overwrite);
}
