using Dalamud.Plugin.Services;

namespace ModOrganizer.Commands;

public class CommandPrinter(IChatGui? maybeChatGui) : ICommandPrinter
{
    public void PrintError(string message) => maybeChatGui?.PrintError(message);
}
