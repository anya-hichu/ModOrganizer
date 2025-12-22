using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Mods;

public interface IModProcessor
{
    bool TryProcess(string modDirectory, [NotNullWhen(true)] out string? newModPath, bool dryRun = false);
}
