using Dalamud.Plugin.Services;
using ModOrganizer.Mods;
using ModOrganizer.Scriban;
using Scriban;
using Scriban.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModOrganizer.Windows.States;

public class EvaluationState(ModInterop modInterop, IPluginLog pluginLog) : ResultState(modInterop, pluginLog)
{
    public string Expression { get; set; } = string.Empty;
    public string ModDirectoryFilter { get; set; } = string.Empty;
    public string ResultFilter { get; set; } = string.Empty;

    public Task Evaluate(IEnumerable<string> modDirectories) => RunTask(cancellationTokenSource =>
    {
        Results.Clear();
        Results = modDirectories.ToDictionary(d => d, modDirectory =>
        {
            if (cancellationTokenSource.IsCancellationRequested) throw new TaskCanceledException($"Task [{Task.CurrentId}] has been canceled inside [{nameof(Evaluate)}] before processing mod [{modDirectory}]");

            if (!ModInterop.TryGetModInfo(modDirectory, out var modInfo)) return new ArgumentException("Failed to retrieve mod info");

            // Need to keep context for string conversion
            var templateContext = new TemplateContext() { MemberRenamer = MemberRenamer.Rename };

            var scriptObject = new ScriptObject();
            scriptObject.Import(modInfo);
            templateContext.PushGlobal(scriptObject);

            try
            {
                var result = Template.Evaluate(Expression, templateContext);
                return (object)templateContext.ObjectToString(result);
            }
            catch (Exception e)
            {
                PluginLog.Warning($"Caught exception while evaluating expression [{Expression}] for mod [{modDirectory}]:\n\t{e.Message}");
                return new ArgumentException("Failed to evaluate", e);
            }
        });
    });

    public override void Clear()
    {
        base.Clear();
        Expression = string.Empty;
        ModDirectoryFilter = string.Empty;
        ResultFilter = string.Empty;
    }
}
