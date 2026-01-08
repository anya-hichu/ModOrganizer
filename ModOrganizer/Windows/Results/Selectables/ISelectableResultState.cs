using System.Collections.Generic;

namespace ModOrganizer.Windows.Results.Selectables;

public interface ISelectableResultState
{
    IEnumerable<ISelectableResult> GetResults<ISelectableResult>();
}
