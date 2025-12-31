using Microsoft.QualityTools.Testing.Fakes.Stubs;

namespace ModOrganizer.Tests.Dalamuds.PluginLogs;

public static class AssertPluginLog
{
    public static void MatchObservedCall(StubObservedCall call, string methodName, Action<string?> matchMessage)
    {
        Assert.AreEqual(methodName, call.StubbedMethod.Name);

        var arguments = call.GetArguments();
        Assert.HasCount(2, arguments);

        matchMessage(arguments[0] as string);
    }
}
