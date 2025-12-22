using System;
using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Backups;

public interface IBackupManager : IDisposable
{
    Backup Create(bool auto = false);
    bool TryCreate([NotNullWhen(true)] out Backup? backup, bool auto = false);
    
    void CreateRecent(bool auto = false);
    
    bool TryRestore(Backup backup, bool reloadPenumbra = false);
    bool TryDelete(Backup backup);

    string GetPath(Backup backup);
    string GetFileName(Backup backup);

    string GetFolderPath();
}
