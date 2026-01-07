using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModOrganizer.Mods;

public interface IModAutoProcessor : IDisposable
{
    IEnumerable<Task> GetRunningTasks();
}
