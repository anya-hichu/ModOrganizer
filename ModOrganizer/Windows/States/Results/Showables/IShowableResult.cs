namespace ModOrganizer.Windows.States.Results.Showables;

public interface IShowableResult<T> where T : IShowableResultState
{
    bool IsShowed(T state);
}
