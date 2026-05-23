using MediaCatalog.Abstractions.Models;

namespace MediaCatalog.Patterns.Strategy;

/// <summary>
/// Strategy pattern: interchangeable algorithms for persisting the catalog to disk.
/// </summary>
public interface ICatalogPersistenceStrategy
{
    void Save(IReadOnlyList<MediaItem> items, string filePath);
    IReadOnlyList<MediaItem> Load(string filePath);
}
