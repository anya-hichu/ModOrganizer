using Microsoft.QualityTools.Testing.Fakes.Stubs;

namespace ModOrganizer.Tests.Systems;

public static class StubAction
{
    public static Action WithObserver(IStubObserver observer) => () =>
    {
        var noop = () => { };
        observer.Enter(noop.GetType(), noop);
    }; 

    public static Action<T1> WithObserver<T1>(IStubObserver observer) => arg1 =>
    {
        var noop = (T1 _) => { };
        observer.Enter(noop.GetType(), noop, arg1);
    };

    public static Action<T1, T2> WithObserver<T1, T2>(IStubObserver observer) => (arg1, arg2) =>
    {
        var noop = (T1 _, T2 __) => { };
        observer.Enter(noop.GetType(), noop, arg1, arg2);
    };
}
