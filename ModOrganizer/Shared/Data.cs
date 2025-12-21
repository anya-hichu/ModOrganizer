using Scriban.Helpers;

namespace ModOrganizer.Json;

public abstract partial class Data
{
    public override string ToString() => GetType().ScriptPrettyName();
}
