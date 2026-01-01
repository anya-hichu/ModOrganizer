using Microsoft.QualityTools.Testing.Fakes.Stubs;

namespace ModOrganizer.Tests.Systems;

public static class FuncDecorator
{
    public static Func<T1, T2, T3, TResult> WithObserver<T1, T2, T3, TResult>(IStubObserver observer, Func<T1, T2, T3, TResult> func) => (T1 arg1, T2 arg2, T3 arg3) =>
    {
        observer.Enter(func.GetType(), func, arg1, arg2, arg3);
        return func.Invoke(arg1, arg2, arg3);
    };
}
