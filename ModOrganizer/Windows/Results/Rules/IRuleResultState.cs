using ModOrganizer.Windows.Results.Selectables;
using ModOrganizer.Windows.Results.Showables;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModOrganizer.Windows.Results.Rules;

public interface IRuleResultState : IResultState, ISelectableResultState, IShowableRuleResultState
{
    event Action? OnResultsChanged;

    new string DirectoryFilter { get; set; }

    new bool ShowErrors { get; set; }
    new bool ShowSamePaths { get; set; }

    new string PathFilter { get; set; }
    new string NewPathFilter { get; set; }

    Task Preview(HashSet<string> modDirectories);
    Task Apply();

    new IEnumerable<T> GetResults<T>();
}
