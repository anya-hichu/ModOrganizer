using System.Collections.Generic;

namespace ModOrganizer.Windows.States.Results.Visibles;

public interface IVisibleResultState
{
    bool ShowErrors { get; }
    bool ShowUnchanging { get; }

    IReadOnlyDictionary<string, IVisibleResult> GetResultByModDirectory<IVisibleResult>();
}
