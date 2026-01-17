using ModOrganizer.Rules;
using ModOrganizer.Windows.Results.Showables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModOrganizer.Windows.Results.Evaluations;

public interface IEvaluationResultState : IResultState, IShowableEvaluationResultState
{
    new string DirectoryFilter { get; set; }

    string Expression { get; set; }
    string Template { get; set; }

    new string ExpressionFilter { get; set; }
    new string TemplateFilter { get; set; }

    Task Evaluate(HashSet<string> modDirectories);
    void Load(Rule rule);
}
