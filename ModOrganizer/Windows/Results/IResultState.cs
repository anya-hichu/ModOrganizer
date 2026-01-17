using System;
using System.Collections.Generic;

namespace ModOrganizer.Windows.Results;

public interface IResultState : IDisposable
{
    string DirectoryFilter { get; set; }

    void Clear();
    bool HasCompletedTask();
    IEnumerable<Result> GetResults();
    IEnumerable<T> GetResults<T>();
}
