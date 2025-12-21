using Dalamud.Plugin.Services;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ModOrganizer.Shared;

public abstract class Converter<I, O>(IPluginLog pluginLog) where I : class where O : class
{
    protected IPluginLog PluginLog { get; init; } = pluginLog;

    public abstract bool TryConvert(I input, [NotNullWhen(true)] out O? output);

    public bool TryConvertMany(IEnumerable<I> inputs, [NotNullWhen(true)] out IEnumerable<O>? outputs)
    {
        outputs = null;
        var list = new List<O>();

        foreach (var input in inputs)
        {
            if (!TryConvert(input, out var output))
            {
                PluginLog.Debug($"Failed to convert one or more [{typeof(I).Name}] to [{typeof(O).Name}]");
                return false;
            }
            list.Add(output);
        }

        outputs = list;
        return true;
    }
}
