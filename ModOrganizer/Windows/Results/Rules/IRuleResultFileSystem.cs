using ModOrganizer.Virtuals;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Windows.Results.Rules;

public interface IRuleResultFileSystem : IVirtualFileSystem, IDisposable
{
    bool TryGetFileData(VirtualFile file, [NotNullWhen(true)] out RulePathResult? fileData);
}
