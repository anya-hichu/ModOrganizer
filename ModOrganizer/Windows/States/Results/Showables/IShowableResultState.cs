using System.Collections.Generic;

namespace ModOrganizer.Windows.States.Results.Showables;

public interface IShowableResultState
{
    string DirectoryFilter { get; }

    IEnumerable<T> GetResults<T>();
}
