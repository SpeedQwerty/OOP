namespace MediaCatalog.Patterns.Observer;

/// <summary>
/// Observer pattern: reacts to catalog lifecycle notifications from <see cref="CatalogSubject"/>.
/// </summary>
public interface ICatalogObserver
{
    void OnCatalogEvent(CatalogEventArgs args);
}

/// <summary>Notification payload for catalog changes.</summary>
public sealed class CatalogEventArgs
{
    public CatalogEventKind Kind { get; init; }
    public string Message { get; init; } = string.Empty;
}

/// <summary>Kinds of catalog events published to observers.</summary>
public enum CatalogEventKind
{
    ItemAdded,
    ItemRemoved,
    Saved,
    Loaded,
    Status
}
