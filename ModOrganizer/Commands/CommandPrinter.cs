using Dalamud.Plugin.Services;

namespace ModOrganizer.Commands;

public class CommandPrinter(IChatGui? chatGui) : ICommandPrinter
{
    public void PrintError(string message) => chatGui?.PrintError(message);
}
