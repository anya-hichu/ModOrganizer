using System.Collections.Generic;

namespace ModOrganizer.Windows.States.Results.Selectables;

public interface ISelectableResultState
{
    IEnumerable<ISelectableResult> GetSelectableResults();
}
