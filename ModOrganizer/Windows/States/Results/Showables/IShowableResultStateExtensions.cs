using System.Collections.Generic;
using System.Linq;

namespace ModOrganizer.Windows.States.Results.Showables;

public static class IShowableResultStateExtensions
{
    public static IReadOnlyDictionary<string, R> GetShowedResultByModDirectory<R, S>(this S state) where R : IShowableResult<S> where S : IShowableResultState => state.GetResultByModDirectory<R>().Where(p => p.Value.IsShowed(state)).ToDictionary();
}

