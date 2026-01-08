using System.Collections.Generic;
using System.Linq;

namespace ModOrganizer.Windows.Results.Showables;

public static class IShowableResultStateExtensions
{
    public static IEnumerable<R> GetShowedResults<R, S>(this S state) where R : IShowableResult<S> where S : IShowableResultState => state.GetResults<R>().Where(r => r.IsShowed(state));
}

