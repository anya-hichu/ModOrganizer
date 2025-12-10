using System.Collections.Generic;
using System.Linq;

namespace ModOrganizer.Windows.States.Results.Selectables;

public static class ISelectableResultStateExtensions
{
    public static void InvertResultSelection(this ISelectableResultState selectableResultState)
    {
        foreach (var selectableResult in selectableResultState.GetSelectableResults()) selectableResult.IsSelected = !selectableResult.IsSelected;
    }

    public static void ClearResultSelection(this ISelectableResultState selectableResultState)
    {
        foreach (var selectedResult in selectableResultState.GetSelectedResults()) selectedResult.IsSelected = false;
    }

    public static IEnumerable<ISelectableResult> GetSelectedResults(this ISelectableResultState selectableResultState) => selectableResultState.GetSelectableResults().Where(r => r.IsSelected);
}
