using Microsoft.QualityTools.Testing.Fakes.Stubs;

namespace ModOrganizer.Tests.Rules.RuleEvaluators;

public static class IStubbableRuleEvaluatorExtensions
{
    public static T WithPluginLogDefaults<T>(this T stubbable) where T : IStubbableRuleEvaluator
    {
        stubbable.RuleEvaluatorStub.BehaveAsDefaultValue();

        return stubbable;
    }

    public static T WithPluginLogObserver<T>(this T stubbable, IStubObserver observer) where T : IStubbableRuleEvaluator
    {
        stubbable.RuleEvaluatorStub.InstanceObserver = observer;

        return stubbable;
    }

}
