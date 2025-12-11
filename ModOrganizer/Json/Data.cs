using Scriban.Helpers;

namespace ModOrganizer.Json;

public abstract class Data
{
    public override string ToString() => GetType().ScriptPrettyName();
}
