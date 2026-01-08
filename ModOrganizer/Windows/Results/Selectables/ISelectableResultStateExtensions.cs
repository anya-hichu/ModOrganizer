using System.Collections.Generic;
using System.Linq;

namespace ModOrganizer.Windows.Results.Selectables;

public static class ISelectableResultStateExtensions
{
    public static void InvertResultSelection(this ISelectableResultState selectableResultState)
    {
        foreach (var selectableResult in selectableResultState.GetSelectableResults()) selectableResult.Selected = !selectableResult.Selected;
    }

    public static void ClearResultSelection(this ISelectableResultState selectableResultState)
    {
        foreach (var selectedResult in selectableResultState.GetSelectedResults()) selectedResult.Selected = false;
    }

    public static IEnumerable<ISelectableResult> GetSelectableResults(this ISelectableResultState selectableResultState) => selectableResultState.GetResults<ISelectableResult>();
    public static IEnumerable<ISelectableResult> GetSelectedResults(this ISelectableResultState selectableResultState) => selectableResultState.GetSelectableResults().Where(r => r.Selected);
}
