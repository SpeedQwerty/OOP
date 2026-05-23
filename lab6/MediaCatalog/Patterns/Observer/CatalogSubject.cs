namespace MediaCatalog.Patterns.Observer;

/// <summary>
/// Observer pattern subject: maintains observers and broadcasts catalog events.
/// Centralizes status updates so the view model does not scatter notification logic.
/// </summary>
public sealed class CatalogSubject
{
    private readonly List<ICatalogObserver> _observers = new();

    public void Attach(ICatalogObserver observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);
    }

    public void Detach(ICatalogObserver observer) => _observers.Remove(observer);

    public void Notify(CatalogEventKind kind, string message)
    {
        var args = new CatalogEventArgs { Kind = kind, Message = message };
        foreach (var observer in _observers.ToArray())
            observer.OnCatalogEvent(args);
    }
}
