namespace ModOrganizer.Windows.States.Results.Selectables;

public static class ISelectableResultExtensions
{
    public static void InvertSelected(this ISelectableResult selectableResult) => selectableResult.Selected = !selectableResult.Selected;
}
