using ModOrganizer.Windows.States.Results.Visibles;
using System.Collections.Generic;

namespace ModOrganizer.Windows.States.Results.Selectables;

public interface ISelectableResultState
{
    IReadOnlyDictionary<string, ISelectableResult> GetResultByModDirectory<ISelectableResult>();
}
