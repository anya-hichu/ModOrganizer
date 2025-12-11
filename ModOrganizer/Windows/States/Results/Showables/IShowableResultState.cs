using System.Collections.Generic;

namespace ModOrganizer.Windows.States.Results.Showables;

public interface IShowableResultState
{
    IReadOnlyDictionary<string, T> GetResultByModDirectory<T>();
}
