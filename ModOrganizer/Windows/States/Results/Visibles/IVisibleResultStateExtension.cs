using System.Collections.Generic;
using System.Linq;

namespace ModOrganizer.Windows.States.Results.Visibles;

public static class IVisibleResultStateExtension
{
    public static IReadOnlyDictionary<string, IVisibleResult> GetVisibleResultByModDirectory(this IVisibleResultState visibleResultState) => visibleResultState.GetResultByModDirectory<IVisibleResult>().Where(p => p.Value.IsVisible(visibleResultState)).ToDictionary();
}
