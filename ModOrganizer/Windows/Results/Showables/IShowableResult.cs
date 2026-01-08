namespace ModOrganizer.Windows.Results.Showables;

public interface IShowableResult<T> where T : IShowableResultState
{
    bool IsShowed(T state);
}
