using Dalamud.Plugin.Services;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace ModOrganizer.Shared;

public abstract class Converter<I, O>(IPluginLog pluginLog) : IConverter<I, O> where I : class where O : class
{
    protected IPluginLog PluginLog { get; init; } = pluginLog;

    public abstract bool TryConvert(I input, [NotNullWhen(true)] out O? output);

    public bool TryConvertMany(IEnumerable<I> inputs, [NotNullWhen(true)] out HashSet<O>? outputs)
    {
        outputs = null;

        var converted = new HashSet<O>();
        foreach (var input in inputs)
        {
            if (!TryConvert(input, out var output))
            {
                PluginLog.Debug($"Failed to convert one or more [{typeof(I).Name}] to [{typeof(O).Name}]");
                return false;
            }
            converted.Add(output);
        }

        var countDiff = inputs.Count() - converted.Count;
        if (countDiff > 0) PluginLog.Warning($"Found {countDiff} duplication(s) while converting, ignoring");

        outputs = converted;
        return true;
    }
}
