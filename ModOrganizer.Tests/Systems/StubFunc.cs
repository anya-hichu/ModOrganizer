using Microsoft.QualityTools.Testing.Fakes.Stubs;

namespace ModOrganizer.Tests.Systems;

public static class StubFunc
{
    public static Func<TResult> WithObserver<TResult>(IStubObserver observer, Func<TResult> stubFunc) => () =>
    {
        observer.Enter(stubFunc.GetType(), stubFunc);
        return stubFunc.Invoke();
    };

    public static Func<T1, T2, T3, TResult> WithObserver<T1, T2, T3, TResult>(IStubObserver observer, Func<T1, T2, T3, TResult> stubFunc) => (T1 arg1, T2 arg2, T3 arg3) =>
    {
        observer.Enter(stubFunc.GetType(), stubFunc, arg1, arg2, arg3);
        return stubFunc.Invoke(arg1, arg2, arg3);
    };
}
