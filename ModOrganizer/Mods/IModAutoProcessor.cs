using System;
using System.Threading.Tasks;

namespace ModOrganizer.Mods;

public interface IModAutoProcessor : IDisposable
{
    Task GetCurrentTask();
}
