using Microsoft.QualityTools.Testing.Fakes.Stubs;

namespace ModOrganizer.Tests.Systems;

public static class ActionDecorator
{
    public static Action<T1> WithObserver<T1>(IStubObserver observer, Action<T1> action) => (T1 arg1) =>
    {
        observer.Enter(action.GetType(), action, arg1);
        action.Invoke(arg1);
    };

    public static Action<T1, T2> WithObserver<T1, T2>(IStubObserver observer, Action<T1, T2> action) => (T1 arg1, T2 arg2) =>
    {
        observer.Enter(action.GetType(), action, arg1, arg2);
        action.Invoke(arg1, arg2);
    };
}
