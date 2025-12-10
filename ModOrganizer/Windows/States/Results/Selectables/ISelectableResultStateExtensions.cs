using System.Collections.Generic;
using System.Linq;

namespace ModOrganizer.Windows.States.Results.Selectables;

public static class ISelectableResultStateExtensions
{
    public static void InvertResultSelection(this ISelectableResultState selectableResultState)
    {
        foreach (var selectableResult in selectableResultState.GetSelectableResultByModDirectory().Values) selectableResult.IsSelected = !selectableResult.IsSelected;
    }

    public static void ClearResultSelection(this ISelectableResultState selectableResultState)
    {
        foreach (var selectedResult in selectableResultState.GetSelectedResultByModDirectory().Values) selectedResult.IsSelected = false;
    }

    public static IReadOnlyDictionary<string, ISelectableResult> GetSelectableResultByModDirectory(this ISelectableResultState selectableResultState) => selectableResultState.GetResultByModDirectory<ISelectableResult>().ToDictionary();
    public static IReadOnlyDictionary<string, ISelectableResult> GetSelectedResultByModDirectory(this ISelectableResultState selectableResultState) => selectableResultState.GetSelectableResultByModDirectory().Where(r => r.Value.IsSelected).ToDictionary();
}
