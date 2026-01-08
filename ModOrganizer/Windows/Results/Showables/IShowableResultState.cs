using System.Collections.Generic;

namespace ModOrganizer.Windows.Results.Showables;

public interface IShowableResultState
{
    string DirectoryFilter { get; }

    IEnumerable<T> GetResults<T>();
}
