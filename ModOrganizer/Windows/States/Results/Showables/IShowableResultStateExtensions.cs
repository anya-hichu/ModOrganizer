using System.Collections.Generic;
using System.Linq;

namespace ModOrganizer.Windows.States.Results.Showables;

public static class IShowableResultStateExtensions
{
    public static IReadOnlyDictionary<string, T> GetShowedResultByModDirectory<T, Z>(this Z state) where T : IShowableResult<Z> where Z : IShowableResultState => state.GetResultByModDirectory<T>().Where(p => p.Value.IsShowed(state)).ToDictionary();
}

